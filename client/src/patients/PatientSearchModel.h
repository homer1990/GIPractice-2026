#pragma once

#include "../api/PracticeApiClient.h"

#include <QAbstractListModel>
#include <QString>

namespace GIPractice::Client::Patients {

class PatientSearchModel final : public QAbstractListModel
{
    Q_OBJECT

    Q_PROPERTY(QString firstName READ firstName WRITE setFirstName NOTIFY firstNameChanged)
    Q_PROPERTY(QString lastName READ lastName WRITE setLastName NOTIFY lastNameChanged)
    Q_PROPERTY(QString fathersName READ fathersName WRITE setFathersName NOTIFY fathersNameChanged)
    Q_PROPERTY(QString birthDateFrom READ birthDateFrom WRITE setBirthDateFrom NOTIFY birthDateFromChanged)
    Q_PROPERTY(QString birthDateTo READ birthDateTo WRITE setBirthDateTo NOTIFY birthDateToChanged)

    Q_PROPERTY(bool loading READ loading NOTIFY loadingChanged)
    Q_PROPERTY(QString errorMessage READ errorMessage NOTIFY errorMessageChanged)
    Q_PROPERTY(int totalCount READ totalCount NOTIFY totalCountChanged)
    Q_PROPERTY(int count READ count NOTIFY countChanged)
    Q_PROPERTY(int page READ page NOTIFY pageChanged)
    Q_PROPERTY(int pageSize READ pageSize CONSTANT)
    Q_PROPERTY(bool canGoPrevious READ canGoPrevious NOTIFY pageChanged)
    Q_PROPERTY(bool canGoNext READ canGoNext NOTIFY pagingChanged)

public:
    enum Role {
        DisplayNameRole = Qt::UserRole + 1,
        FirstNameRole,
        LastNameRole,
        FathersNameRole,
        BirthDateRole
    };
    Q_ENUM(Role)

    explicit PatientSearchModel(Api::PracticeApiClient *api, QObject *parent = nullptr);

    [[nodiscard]] int rowCount(const QModelIndex &parent = QModelIndex()) const override;
    [[nodiscard]] QVariant data(const QModelIndex &index, int role = Qt::DisplayRole) const override;
    [[nodiscard]] QHash<int, QByteArray> roleNames() const override;

    [[nodiscard]] QString firstName() const;
    void setFirstName(const QString &value);

    [[nodiscard]] QString lastName() const;
    void setLastName(const QString &value);

    [[nodiscard]] QString fathersName() const;
    void setFathersName(const QString &value);

    [[nodiscard]] QString birthDateFrom() const;
    void setBirthDateFrom(const QString &value);

    [[nodiscard]] QString birthDateTo() const;
    void setBirthDateTo(const QString &value);

    [[nodiscard]] bool loading() const noexcept;
    [[nodiscard]] QString errorMessage() const;
    [[nodiscard]] int totalCount() const noexcept;
    [[nodiscard]] int count() const noexcept;
    [[nodiscard]] int page() const noexcept;
    [[nodiscard]] int pageSize() const noexcept;
    [[nodiscard]] bool canGoPrevious() const noexcept;
    [[nodiscard]] bool canGoNext() const noexcept;

    Q_INVOKABLE void search();
    Q_INVOKABLE void clearCriteria();
    Q_INVOKABLE void previousPage();
    Q_INVOKABLE void nextPage();
    Q_INVOKABLE [[nodiscard]] QString patientIdAt(int row) const;

Q_SIGNALS:
    void firstNameChanged();
    void lastNameChanged();
    void fathersNameChanged();
    void birthDateFromChanged();
    void birthDateToChanged();
    void loadingChanged();
    void errorMessageChanged();
    void totalCountChanged();
    void countChanged();
    void pageChanged();
    void pagingChanged();

private:
    Api::PracticeApiClient *m_api = nullptr;
    QList<Api::PatientDto> m_patients;

    QString m_firstName;
    QString m_lastName;
    QString m_fathersName;
    QString m_birthDateFrom;
    QString m_birthDateTo;
    QString m_errorMessage;

    bool m_loading = false;
    int m_totalCount = 0;
    int m_page = 1;
    static constexpr int PageSize = 50;
    quint64 m_requestSerial = 0;

    void requestPage(int page);
    void setLoading(bool value);
    void setErrorMessage(QString value);
    void replaceResults(Api::PatientSearchResponse response);
    [[nodiscard]] bool buildRequest(Api::PatientSearchRequest &request);
};

} // namespace GIPractice::Client::Patients
