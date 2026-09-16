#pragma once

#include "AnatomyVocabulary.h"

#include <QAbstractListModel>
#include <QLocale>
#include <QString>

namespace GIPractice::Client::Clinical {

// Thin QML-facing adapter over AnatomyVocabulary.
//
// QML receives localized presentation fields only. The canonical concept code is not a
// display role; codeAt() is used explicitly when the user accepts a suggestion and the
// client needs to persist the semantic selection.
class AnatomySuggestionModel final : public QAbstractListModel
{
    Q_OBJECT
    Q_PROPERTY(QString query READ query WRITE setQuery NOTIFY queryChanged)
    Q_PROPERTY(QString localeName READ localeName WRITE setLocaleName NOTIFY localeNameChanged)
    Q_PROPERTY(int limit READ limit WRITE setLimit NOTIFY limitChanged)

public:
    enum Role {
        DisplayNameRole = Qt::UserRole + 1,
        MatchedTextRole,
        MatchedLocaleRole,
        KindRole,
        ExactMatchRole
    };
    Q_ENUM(Role)

    explicit AnatomySuggestionModel(QObject *parent = nullptr);

    [[nodiscard]] int rowCount(const QModelIndex &parent = QModelIndex()) const override;
    [[nodiscard]] QVariant data(const QModelIndex &index, int role = Qt::DisplayRole) const override;
    [[nodiscard]] QHash<int, QByteArray> roleNames() const override;

    [[nodiscard]] QString query() const;
    void setQuery(const QString &query);

    [[nodiscard]] QString localeName() const;
    void setLocaleName(const QString &localeName);

    [[nodiscard]] int limit() const noexcept;
    void setLimit(int limit);

    // Called by C++ service/API code when vocabulary data has been loaded or refreshed.
    // It is deliberately not Q_INVOKABLE: QML consumes the vocabulary; it does not own it.
    void setEntries(QList<AnatomicalSiteEntry> entries);

    // Semantic identity is requested explicitly on acceptance instead of being exposed as
    // ordinary display data in every delegate.
    Q_INVOKABLE [[nodiscard]] QString codeAt(int row) const;

signals:
    void queryChanged();
    void localeNameChanged();
    void limitChanged();

private:
    AnatomyVocabulary m_vocabulary;
    QList<AnatomicalSiteSuggestion> m_suggestions;
    QString m_query;
    QLocale m_locale = QLocale(QStringLiteral("el_GR"));
    int m_limit = 8;

    void refresh();
};

} // namespace GIPractice::Client::Clinical
