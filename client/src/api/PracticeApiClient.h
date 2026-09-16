#pragma once

#include <QByteArray>
#include <QDate>
#include <QDateTime>
#include <QList>
#include <QObject>
#include <QString>
#include <QUuid>
#include <QUrl>

#include <functional>
#include <variant>

class QNetworkAccessManager;

namespace GIPractice::Client::Api {

struct PatientDto
{
    QUuid id;
    QString firstName;
    QString lastName;
    QString fathersName;
    QDate birthDate;
};

struct PatientSearchResponse
{
    QList<PatientDto> items;
    int totalCount = 0;
    int page = 1;
    int pageSize = 50;
};

struct AppointmentDto
{
    QUuid id;
    QUuid patientId;
    QString patientDisplayName;
    QString typeCode;
    QDateTime startUtc;
    int durationMinutes = 0;
    QString status;
    QString notes;
};

struct PatientSearchRequest
{
    QString firstName;
    QString lastName;
    QString fathersName;
    QDate birthDateFrom;
    QDate birthDateTo;
    int page = 1;
    int pageSize = 50;
};

struct AppointmentListRequest
{
    QDateTime fromUtc;
    QDateTime toUtc;
    QUuid patientId;
};

struct ApiError
{
    int httpStatus = 0;
    QString message;
    QByteArray responseBody;
};

template<typename T>
using ApiResult = std::variant<T, ApiError>;

class PracticeApiClient final : public QObject
{
public:
    explicit PracticeApiClient(QUrl baseUrl, QObject *parent = nullptr);

    [[nodiscard]] QUrl baseUrl() const;
    void setBaseUrl(QUrl baseUrl);

    void searchPatients(
        const PatientSearchRequest &request,
        std::function<void(ApiResult<PatientSearchResponse>)> completion);

    void getPatient(
        const QUuid &patientId,
        std::function<void(ApiResult<PatientDto>)> completion);

    void listAppointments(
        const AppointmentListRequest &request,
        std::function<void(ApiResult<QList<AppointmentDto>>)> completion);

private:
    QNetworkAccessManager *m_network = nullptr;
    QUrl m_baseUrl;

    [[nodiscard]] QUrl endpointUrl(const QString &relativePath) const;
};

} // namespace GIPractice::Client::Api
