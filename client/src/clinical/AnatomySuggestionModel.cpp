#include "AnatomySuggestionModel.h"

#include <QVariant>

#include <algorithm>
#include <utility>

namespace GIPractice::Client::Clinical {

AnatomySuggestionModel::AnatomySuggestionModel(QObject *parent)
    : QAbstractListModel(parent)
{
}

int AnatomySuggestionModel::rowCount(const QModelIndex &parent) const
{
    if (parent.isValid())
        return 0;
    return m_suggestions.size();
}

QVariant AnatomySuggestionModel::data(const QModelIndex &index, const int role) const
{
    if (!index.isValid() || index.row() < 0 || index.row() >= m_suggestions.size())
        return {};

    const auto &suggestion = m_suggestions.at(index.row());

    switch (role) {
    case Qt::DisplayRole:
    case DisplayNameRole:
        return suggestion.displayName;
    case MatchedTextRole:
        return suggestion.matchedText;
    case MatchedLocaleRole:
        return suggestion.matchedLocale;
    case KindRole:
        return static_cast<int>(suggestion.kind);
    case ExactMatchRole:
        return suggestion.exactMatch;
    default:
        return {};
    }
}

QHash<int, QByteArray> AnatomySuggestionModel::roleNames() const
{
    return {
        { DisplayNameRole, "displayName" },
        { MatchedTextRole, "matchedText" },
        { MatchedLocaleRole, "matchedLocale" },
        { KindRole, "kind" },
        { ExactMatchRole, "exactMatch" }
    };
}

QString AnatomySuggestionModel::query() const
{
    return m_query;
}

void AnatomySuggestionModel::setQuery(const QString &query)
{
    if (m_query == query)
        return;

    m_query = query;
    Q_EMIT queryChanged();
    refresh();
}

QString AnatomySuggestionModel::localeName() const
{
    return m_locale.bcp47Name();
}

void AnatomySuggestionModel::setLocaleName(const QString &localeName)
{
    const auto requested = QLocale(localeName);
    if (requested == m_locale)
        return;

    m_locale = requested;
    Q_EMIT localeNameChanged();
    refresh();
}

int AnatomySuggestionModel::limit() const noexcept
{
    return m_limit;
}

void AnatomySuggestionModel::setLimit(const int limit)
{
    const auto normalized = std::clamp(limit, 1, 100);
    if (normalized == m_limit)
        return;

    m_limit = normalized;
    Q_EMIT limitChanged();
    refresh();
}

void AnatomySuggestionModel::setEntries(QList<AnatomicalSiteEntry> entries)
{
    m_vocabulary.setEntries(std::move(entries));
    refresh();
}

QString AnatomySuggestionModel::codeAt(const int row) const
{
    if (row < 0 || row >= m_suggestions.size())
        return {};
    return m_suggestions.at(row).code;
}

void AnatomySuggestionModel::refresh()
{
    QList<AnatomicalSiteSuggestion> next;
    if (!m_query.trimmed().isEmpty())
        next = m_vocabulary.suggest(m_query, m_locale, m_limit);

    beginResetModel();
    m_suggestions = std::move(next);
    endResetModel();
}

} // namespace GIPractice::Client::Clinical
