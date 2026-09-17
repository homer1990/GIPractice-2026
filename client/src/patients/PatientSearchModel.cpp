#include "PatientSearchModel.h"

#include <KLocalizedString>

#include <QDate>
#include <QPointer>
#include <QVariant>

#include <utility>
#include <variant>

namespace GIPractice::Client::Patients {

PatientSearchModel::PatientSearchModel(Api::PracticeApiClient *api, QObject *parent)
    : QAbstractListModel(parent)
    , m_api(api)
{
}

int PatientSearchModel::rowCount(const QModelIndex &parent) const
{
    if (parent.isValid())
        return 0;
    return m_patients.size();
}

QVariant PatientSearchModel::data(const QModelIndex &index, const int role) const
{
    if (!index.isValid() || index.row() < 0 || index.row() >= m_patients.size())
        return {};

    const auto &patient = m_patients.at(index.row());

    switch (role) {
    case Qt::DisplayRole:
    case DisplayNameRole:
        return QStringLiteral("%1 %2").arg(patient.lastName, patient.firstName);
    case FirstNameRole:
        return patient.firstName;
    case LastNameRole:
        return patient.lastName;
    case FathersNameRole:
        return patient.fathersName;
    case BirthDateRole:
        return patient.birthDate.isValid() ? patient.birthDate.toString(Qt::ISODate) : QString{};
    default:
        return {};
    }
}

QHash<int, QByteArray> PatientSearchModel::roleNames() const
{
    return {
        { DisplayNameRole, "displayName" },
        { FirstNameRole, "firstName" },
        { LastNameRole, "lastName" },
        { FathersNameRole, "fathersName" },
        { BirthDateRole, "birthDate" }
    };
}

QString PatientSearchModel::firstName() const
{
    return m_firstName;
}

void PatientSearchModel::setFirstName(const QString &value)
{
    if (m_firstName == value)
        return;
    m_firstName = value;
    Q_EMIT firstNameChanged();
}

QString PatientSearchModel::lastName() const
{
    return m_lastName;
}

void PatientSearchModel::setLastName(const QString &value)
{
    if (m_lastName == value)
        return;
    m_lastName = value;
    Q_EMIT lastNameChanged();
}

QString PatientSearchModel::fathersName() const
{
    return m_fathersName;
}

void PatientSearchModel::setFathersName(const QString &value)
{
    if (m_fathersName == value)
        return;
    m_fathersName = value;
    Q_EMIT fathersNameChanged();
}

QString PatientSearchModel::birthDateFrom() const
{
    return m_birthDateFrom;
}

void PatientSearchModel::setBirthDateFrom(const QString &value)
{
    if (m_birthDateFrom == value)
        return;
    m_birthDateFrom = value;
    Q_EMIT birthDateFromChanged();
}

QString PatientSearchModel::birthDateTo() const
{
    return m_birthDateTo;
}

void PatientSearchModel::setBirthDateTo(const QString &value)
{
    if (m_birthDateTo == value)
        return;
    m_birthDateTo = value;
    Q_EMIT birthDateToChanged();
}

bool PatientSearchModel::loading() const noexcept
{
    return m_loading;
}

QString PatientSearchModel::errorMessage() const
{
    return m_errorMessage;
}

int PatientSearchModel::totalCount() const noexcept
{
    return m_totalCount;
}

int PatientSearchModel::count() const noexcept
{
    return m_patients.size();
}

int PatientSearchModel::page() const noexcept
{
    return m_page;
}

int PatientSearchModel::pageSize() const noexcept
{
    return PageSize;
}

bool PatientSearchModel::canGoPrevious() const noexcept
{
    return m_page > 1;
}

bool PatientSearchModel::canGoNext() const noexcept
{
    return m_page * PageSize < m_totalCount;
}

void PatientSearchModel::search()
{
    requestPage(1);
}

void PatientSearchModel::clearCriteria()
{
    setFirstName({});
    setLastName({});
    setFathersName({});
    setBirthDateFrom({});
    setBirthDateTo({});
    search();
}

void PatientSearchModel::previousPage()
{
    if (!m_loading && canGoPrevious())
        requestPage(m_page - 1);
}

void PatientSearchModel::nextPage()
{
    if (!m_loading && canGoNext())
        requestPage(m_page + 1);
}

QString PatientSearchModel::patientIdAt(const int row) const
{
    if (row < 0 || row >= m_patients.size())
        return {};
    return m_patients.at(row).id.toString(QUuid::WithoutBraces);
}

void PatientSearchModel::requestPage(const int requestedPage)
{
    if (m_api == nullptr) {
        setErrorMessage(i18n("Δεν έχει οριστεί διεύθυνση API. Ορίστε τη μεταβλητή GIPRACTICE_API_URL."));
        return;
    }

    Api::PatientSearchRequest request;
    if (!buildRequest(request))
        return;

    request.page = requestedPage;
    request.pageSize = PageSize;

    setErrorMessage({});
    setLoading(true);

    const auto serial = ++m_requestSerial;
    QPointer<PatientSearchModel> self(this);

    m_api->searchPatients(request, [self, serial](auto result) mutable {
        if (self.isNull() || serial != self->m_requestSerial)
            return;

        self->setLoading(false);

        if (auto *response = std::get_if<Api::PatientSearchResponse>(&result)) {
            self->replaceResults(std::move(*response));
            return;
        }

        const auto &error = std::get<Api::ApiError>(result);
        if (error.httpStatus > 0) {
            self->setErrorMessage(
                i18n("Η αναζήτηση απέτυχε (HTTP %1): %2", error.httpStatus, error.message));
        } else {
            self->setErrorMessage(i18n("Η αναζήτηση απέτυχε: %1", error.message));
        }
    });
}

void PatientSearchModel::setLoading(const bool value)
{
    if (m_loading == value)
        return;
    m_loading = value;
    Q_EMIT loadingChanged();
}

void PatientSearchModel::setErrorMessage(QString value)
{
    if (m_errorMessage == value)
        return;
    m_errorMessage = std::move(value);
    Q_EMIT errorMessageChanged();
}

void PatientSearchModel::replaceResults(Api::PatientSearchResponse response)
{
    const auto oldCount = m_patients.size();
    const auto oldTotalCount = m_totalCount;
    const auto oldPage = m_page;

    beginResetModel();
    m_patients = std::move(response.items);
    endResetModel();

    m_totalCount = response.totalCount;
    m_page = response.page;

    if (oldCount != m_patients.size())
        Q_EMIT countChanged();
    if (oldTotalCount != m_totalCount)
        Q_EMIT totalCountChanged();
    if (oldPage != m_page)
        Q_EMIT pageChanged();

    Q_EMIT pagingChanged();
}

bool PatientSearchModel::buildRequest(Api::PatientSearchRequest &request)
{
    request.firstName = m_firstName;
    request.lastName = m_lastName;
    request.fathersName = m_fathersName;

    const auto parseOptionalDate = [this](
                                       const QString &text,
                                       const QString &fieldName,
                                       QDate &target) -> bool {
        const auto trimmed = text.trimmed();
        if (trimmed.isEmpty()) {
            target = {};
            return true;
        }

        target = QDate::fromString(trimmed, Qt::ISODate);
        if (target.isValid())
            return true;

        setErrorMessage(i18n("Το πεδίο «%1» πρέπει να είναι ημερομηνία της μορφής YYYY-MM-DD.", fieldName));
        return false;
    };

    if (!parseOptionalDate(m_birthDateFrom, i18n("Ημερομηνία γέννησης από"), request.birthDateFrom))
        return false;
    if (!parseOptionalDate(m_birthDateTo, i18n("Ημερομηνία γέννησης έως"), request.birthDateTo))
        return false;

    if (request.birthDateFrom.isValid()
        && request.birthDateTo.isValid()
        && request.birthDateFrom > request.birthDateTo) {
        setErrorMessage(i18n("Η αρχική ημερομηνία γέννησης δεν μπορεί να είναι μετά την τελική."));
        return false;
    }

    return true;
}

} // namespace GIPractice::Client::Patients
