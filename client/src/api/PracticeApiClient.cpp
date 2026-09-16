#include "PracticeApiClient.h"

#include <QJsonArray>
#include <QJsonDocument>
#include <QJsonObject>
#include <QJsonParseError>
#include <QNetworkAccessManager>
#include <QNetworkReply>
#include <QNetworkRequest>
#include <QUrlQuery>

#include <utility>

namespace GIPractice::Client::Api {
namespace {

ApiError replyError(QNetworkReply *reply, const QByteArray &body)
{
    return ApiError{
        reply->attribute(QNetworkRequest::HttpStatusCodeAttribute).toInt(),
        reply->errorString(),
        body};
}

ApiError parseError(const QString &message, const QByteArray &body)
{
    return ApiError{0, message, body};
}

bool parsePatient(const QJsonObject &object, PatientDto &patient)
{
    patient.id = QUuid(object.value(QStringLiteral("id")).toString());
    patient.firstName = object.value(QStringLiteral("firstName")).toString();
    patient.lastName = object.value(QStringLiteral("lastName")).toString();
    patient.fathersName = object.value(QStringLiteral("fathersName")).toString();

    const auto birthDateText = object.value(QStringLiteral("birthDate")).toString();
    patient.birthDate = birthDateText.isEmpty()
        ? QDate{}
        : QDate::fromString(birthDateText, Qt::ISODate);

    return !patient.id.isNull()
        && !patient.firstName.isEmpty()
        && !patient.lastName.isEmpty();
}

bool parseAppointment(const QJsonObject &object, AppointmentDto &appointment)
{
    appointment.id = QUuid(object.value(QStringLiteral("id")).toString());
    appointment.patientId = QUuid(object.value(QStringLiteral("patientId")).toString());
    appointment.patientDisplayName = object.value(QStringLiteral("patientDisplayName")).toString();
    appointment.typeCode = object.value(QStringLiteral("typeCode")).toString();
    appointment.durationMinutes = object.value(QStringLiteral("durationMinutes")).toInt();
    appointment.status = object.value(QStringLiteral("status")).toString();
    appointment.notes = object.value(QStringLiteral("notes")).toString();

    const auto startText = object.value(QStringLiteral("startUtc")).toString();
    appointment.startUtc = QDateTime::fromString(startText, Qt::ISODateWithMs);
    if (!appointment.startUtc.isValid())
        appointment.startUtc = QDateTime::fromString(startText, Qt::ISODate);

    if (appointment.startUtc.isValid())
        appointment.startUtc = appointment.startUtc.toUTC();

    return !appointment.id.isNull()
        && !appointment.patientId.isNull()
        && !appointment.patientDisplayName.isEmpty()
        && !appointment.typeCode.isEmpty()
        && appointment.startUtc.isValid()
        && appointment.durationMinutes > 0;
}

} // namespace

PracticeApiClient::PracticeApiClient(QUrl baseUrl, QObject *parent)
    : QObject(parent)
    , m_network(new QNetworkAccessManager(this))
{
    setBaseUrl(std::move(baseUrl));
}

QUrl PracticeApiClient::baseUrl() const
{
    return m_baseUrl;
}

void PracticeApiClient::setBaseUrl(QUrl baseUrl)
{
    auto path = baseUrl.path();
    if (path.isEmpty())
        path = QStringLiteral("/");
    else if (!path.endsWith(u'/'))
        path.append(u'/');

    baseUrl.setPath(path);
    m_baseUrl = std::move(baseUrl);
}

void PracticeApiClient::searchPatients(
    const PatientSearchRequest &request,
    std::function<void(ApiResult<PatientSearchResponse>)> completion)
{
    auto url = endpointUrl(QStringLiteral("api/patients/search"));
    QUrlQuery query;

    const auto addText = [&query](const QString &name, const QString &value) {
        const auto trimmed = value.trimmed();
        if (!trimmed.isEmpty())
            query.addQueryItem(name, trimmed);
    };

    addText(QStringLiteral("firstName"), request.firstName);
    addText(QStringLiteral("lastName"), request.lastName);
    addText(QStringLiteral("fathersName"), request.fathersName);

    if (request.birthDateFrom.isValid())
        query.addQueryItem(QStringLiteral("birthDateFrom"), request.birthDateFrom.toString(Qt::ISODate));
    if (request.birthDateTo.isValid())
        query.addQueryItem(QStringLiteral("birthDateTo"), request.birthDateTo.toString(Qt::ISODate));

    query.addQueryItem(QStringLiteral("page"), QString::number(request.page));
    query.addQueryItem(QStringLiteral("pageSize"), QString::number(request.pageSize));
    url.setQuery(query);

    auto *reply = m_network->get(QNetworkRequest(url));
    connect(reply, &QNetworkReply::finished, this, [reply, completion = std::move(completion)]() mutable {
        const auto body = reply->readAll();
        if (reply->error() != QNetworkReply::NoError) {
            completion(replyError(reply, body));
            reply->deleteLater();
            return;
        }

        QJsonParseError jsonError;
        const auto document = QJsonDocument::fromJson(body, &jsonError);
        if (jsonError.error != QJsonParseError::NoError || !document.isObject()) {
            completion(parseError(QStringLiteral("Invalid patient-search JSON response."), body));
            reply->deleteLater();
            return;
        }

        const auto root = document.object();
        PatientSearchResponse response;
        response.totalCount = root.value(QStringLiteral("totalCount")).toInt();
        response.page = root.value(QStringLiteral("page")).toInt(1);
        response.pageSize = root.value(QStringLiteral("pageSize")).toInt(50);

        const auto items = root.value(QStringLiteral("items")).toArray();
        response.items.reserve(items.size());
        for (const auto &value : items) {
            if (!value.isObject()) {
                completion(parseError(QStringLiteral("Invalid patient item in JSON response."), body));
                reply->deleteLater();
                return;
            }

            PatientDto patient;
            if (!parsePatient(value.toObject(), patient)) {
                completion(parseError(QStringLiteral("Incomplete patient item in JSON response."), body));
                reply->deleteLater();
                return;
            }
            response.items.append(std::move(patient));
        }

        completion(std::move(response));
        reply->deleteLater();
    });
}

void PracticeApiClient::getPatient(
    const QUuid &patientId,
    std::function<void(ApiResult<PatientDto>)> completion)
{
    const auto id = patientId.toString(QUuid::WithoutBraces);
    auto *reply = m_network->get(QNetworkRequest(endpointUrl(QStringLiteral("api/patients/%1").arg(id))));

    connect(reply, &QNetworkReply::finished, this, [reply, completion = std::move(completion)]() mutable {
        const auto body = reply->readAll();
        if (reply->error() != QNetworkReply::NoError) {
            completion(replyError(reply, body));
            reply->deleteLater();
            return;
        }

        QJsonParseError jsonError;
        const auto document = QJsonDocument::fromJson(body, &jsonError);
        if (jsonError.error != QJsonParseError::NoError || !document.isObject()) {
            completion(parseError(QStringLiteral("Invalid patient JSON response."), body));
            reply->deleteLater();
            return;
        }

        PatientDto patient;
        if (!parsePatient(document.object(), patient)) {
            completion(parseError(QStringLiteral("Incomplete patient JSON response."), body));
            reply->deleteLater();
            return;
        }

        completion(std::move(patient));
        reply->deleteLater();
    });
}

void PracticeApiClient::listAppointments(
    const AppointmentListRequest &request,
    std::function<void(ApiResult<QList<AppointmentDto>>)> completion)
{
    auto url = endpointUrl(QStringLiteral("api/appointments"));
    QUrlQuery query;
    query.addQueryItem(
        QStringLiteral("fromUtc"),
        request.fromUtc.toUTC().toString(Qt::ISODateWithMs));
    query.addQueryItem(
        QStringLiteral("toUtc"),
        request.toUtc.toUTC().toString(Qt::ISODateWithMs));

    if (!request.patientId.isNull()) {
        query.addQueryItem(
            QStringLiteral("patientId"),
            request.patientId.toString(QUuid::WithoutBraces));
    }

    url.setQuery(query);

    auto *reply = m_network->get(QNetworkRequest(url));
    connect(reply, &QNetworkReply::finished, this, [reply, completion = std::move(completion)]() mutable {
        const auto body = reply->readAll();
        if (reply->error() != QNetworkReply::NoError) {
            completion(replyError(reply, body));
            reply->deleteLater();
            return;
        }

        QJsonParseError jsonError;
        const auto document = QJsonDocument::fromJson(body, &jsonError);
        if (jsonError.error != QJsonParseError::NoError || !document.isArray()) {
            completion(parseError(QStringLiteral("Invalid appointment-list JSON response."), body));
            reply->deleteLater();
            return;
        }

        const auto values = document.array();
        QList<AppointmentDto> appointments;
        appointments.reserve(values.size());

        for (const auto &value : values) {
            if (!value.isObject()) {
                completion(parseError(QStringLiteral("Invalid appointment item in JSON response."), body));
                reply->deleteLater();
                return;
            }

            AppointmentDto appointment;
            if (!parseAppointment(value.toObject(), appointment)) {
                completion(parseError(QStringLiteral("Incomplete appointment item in JSON response."), body));
                reply->deleteLater();
                return;
            }
            appointments.append(std::move(appointment));
        }

        completion(std::move(appointments));
        reply->deleteLater();
    });
}

QUrl PracticeApiClient::endpointUrl(const QString &relativePath) const
{
    return m_baseUrl.resolved(QUrl(relativePath));
}

} // namespace GIPractice::Client::Api
