#include "AnatomyVocabulary.h"

#include <QChar>

#include <algorithm>
#include <tuple>
#include <utility>

namespace GIPractice::Client::Clinical {
namespace {

struct RankedSuggestion
{
    AnatomicalSiteSuggestion suggestion;
    int matchRank = 0;   // exact, prefix, contains
    int localeRank = 0;  // requested locale first, then fallbacks
    int sourceRank = 0;  // preferred display name before aliases
};

QString canonicalLocale(QString locale)
{
    return locale.trimmed().replace(u'_', u'-');
}

bool isCombiningMark(const QChar ch)
{
    const auto category = ch.category();
    return category == QChar::Mark_NonSpacing
        || category == QChar::Mark_SpacingCombining
        || category == QChar::Mark_Enclosing;
}

} // namespace

AnatomyVocabulary::AnatomyVocabulary(QList<AnatomicalSiteEntry> entries)
    : m_entries(std::move(entries))
{
}

void AnatomyVocabulary::setEntries(QList<AnatomicalSiteEntry> entries)
{
    m_entries = std::move(entries);
}

const QList<AnatomicalSiteEntry> &AnatomyVocabulary::entries() const noexcept
{
    return m_entries;
}

QString AnatomyVocabulary::displayName(const QString &code, const QLocale &requestedLocale) const
{
    const auto *entry = findByCode(code);
    if (entry == nullptr)
        return code;

    return localizedName(*entry, localeTag(requestedLocale));
}

std::optional<AnatomicalSiteSuggestion> AnatomyVocabulary::resolveExact(
    const QString &text,
    const QLocale &requestedLocale) const
{
    const auto normalizedInput = normalizeForMatch(text);
    if (normalizedInput.isEmpty())
        return std::nullopt;

    const auto requestedTag = localeTag(requestedLocale);
    std::optional<RankedSuggestion> best;

    for (const auto &entry : m_entries) {
        if (normalizeForMatch(entry.code) == normalizedInput) {
            RankedSuggestion candidate{
                AnatomicalSiteSuggestion{
                    entry.code,
                    localizedName(entry, requestedTag),
                    entry.code,
                    {},
                    entry.kind,
                    true},
                0,
                0,
                0};

            if (!best || std::tie(candidate.matchRank, candidate.localeRank, candidate.sourceRank)
                    < std::tie(best->matchRank, best->localeRank, best->sourceRank)) {
                best = std::move(candidate);
            }
        }

        for (const auto &localization : entry.localizations) {
            const auto rank = localeRank(localization.locale, requestedTag);

            const auto consider = [&](const QString &candidateText, const int sourceRank) {
                if (normalizeForMatch(candidateText) != normalizedInput)
                    return;

                RankedSuggestion candidate{
                    AnatomicalSiteSuggestion{
                        entry.code,
                        localizedName(entry, requestedTag),
                        candidateText,
                        localization.locale,
                        entry.kind,
                        true},
                    0,
                    rank,
                    sourceRank};

                if (!best || std::tie(candidate.matchRank, candidate.localeRank, candidate.sourceRank)
                        < std::tie(best->matchRank, best->localeRank, best->sourceRank)) {
                    best = std::move(candidate);
                }
            };

            consider(localization.displayName, 0);
            for (const auto &alias : localization.aliases)
                consider(alias, 1);
        }
    }

    if (!best)
        return std::nullopt;

    return best->suggestion;
}

QList<AnatomicalSiteSuggestion> AnatomyVocabulary::suggest(
    const QString &text,
    const QLocale &requestedLocale,
    const int limit) const
{
    if (limit <= 0)
        return {};

    const auto normalizedInput = normalizeForMatch(text);
    if (normalizedInput.isEmpty())
        return {};

    const auto requestedTag = localeTag(requestedLocale);
    QList<RankedSuggestion> ranked;

    for (const auto &entry : m_entries) {
        std::optional<RankedSuggestion> bestForEntry;

        const auto consider = [&](const QString &candidateText,
                                  const QString &candidateLocale,
                                  const int sourceRank) {
            const auto normalizedCandidate = normalizeForMatch(candidateText);
            if (normalizedCandidate.isEmpty())
                return;

            int matchRank = -1;
            if (normalizedCandidate == normalizedInput)
                matchRank = 0;
            else if (normalizedCandidate.startsWith(normalizedInput))
                matchRank = 1;
            else if (normalizedCandidate.contains(normalizedInput))
                matchRank = 2;

            if (matchRank < 0)
                return;

            RankedSuggestion candidate{
                AnatomicalSiteSuggestion{
                    entry.code,
                    localizedName(entry, requestedTag),
                    candidateText,
                    candidateLocale,
                    entry.kind,
                    matchRank == 0},
                matchRank,
                localeRank(candidateLocale, requestedTag),
                sourceRank};

            if (!bestForEntry
                || std::tie(candidate.matchRank, candidate.localeRank, candidate.sourceRank)
                    < std::tie(bestForEntry->matchRank, bestForEntry->localeRank, bestForEntry->sourceRank)) {
                bestForEntry = std::move(candidate);
            }
        };

        consider(entry.code, {}, 2);
        for (const auto &localization : entry.localizations) {
            consider(localization.displayName, localization.locale, 0);
            for (const auto &alias : localization.aliases)
                consider(alias, localization.locale, 1);
        }

        if (bestForEntry)
            ranked.append(std::move(*bestForEntry));
    }

    std::stable_sort(ranked.begin(), ranked.end(), [](const auto &left, const auto &right) {
        const auto leftKey = std::tie(left.matchRank, left.localeRank, left.sourceRank);
        const auto rightKey = std::tie(right.matchRank, right.localeRank, right.sourceRank);
        if (leftKey != rightKey)
            return leftKey < rightKey;

        return left.suggestion.displayName.localeAwareCompare(right.suggestion.displayName) < 0;
    });

    QList<AnatomicalSiteSuggestion> result;
    result.reserve(std::min(limit, static_cast<int>(ranked.size())));
    for (const auto &item : ranked) {
        if (result.size() >= limit)
            break;
        result.append(item.suggestion);
    }

    return result;
}

QList<AnatomicalSiteEntry> AnatomyVocabulary::childrenOf(const QString &parentCode) const
{
    QList<AnatomicalSiteEntry> result;
    for (const auto &entry : m_entries) {
        if (entry.parentCode == parentCode)
            result.append(entry);
    }

    std::stable_sort(result.begin(), result.end(), [](const auto &left, const auto &right) {
        if (left.sortOrder != right.sortOrder)
            return left.sortOrder < right.sortOrder;
        return left.code < right.code;
    });

    return result;
}

const AnatomicalSiteEntry *AnatomyVocabulary::findByCode(const QString &code) const
{
    const auto it = std::find_if(m_entries.cbegin(), m_entries.cend(), [&](const auto &entry) {
        return entry.code == code;
    });

    return it == m_entries.cend() ? nullptr : &*it;
}

QString AnatomyVocabulary::normalizeForMatch(const QString &text)
{
    const auto decomposed = text.trimmed().normalized(QString::NormalizationForm_D).toCaseFolded();

    QString result;
    result.reserve(decomposed.size());
    bool previousWasSpace = false;

    for (const auto ch : decomposed) {
        if (isCombiningMark(ch))
            continue;

        if (ch.isLetterOrNumber()) {
            result.append(ch);
            previousWasSpace = false;
            continue;
        }

        if (!previousWasSpace && !result.isEmpty()) {
            result.append(u' ');
            previousWasSpace = true;
        }
    }

    return result.trimmed();
}

QString AnatomyVocabulary::localeTag(const QLocale &locale)
{
    return canonicalLocale(locale.bcp47Name());
}

QString AnatomyVocabulary::languageTag(const QString &locale)
{
    const auto canonical = canonicalLocale(locale);
    const auto separator = canonical.indexOf(u'-');
    return (separator < 0 ? canonical : canonical.left(separator)).toCaseFolded();
}

int AnatomyVocabulary::localeRank(const QString &candidateLocale, const QString &requestedLocale)
{
    const auto candidate = canonicalLocale(candidateLocale);
    const auto requested = canonicalLocale(requestedLocale);

    if (!candidate.isEmpty() && candidate.compare(requested, Qt::CaseInsensitive) == 0)
        return 0;

    if (!candidate.isEmpty() && languageTag(candidate) == languageTag(requested))
        return 1;

    if (candidate.compare(QString::fromLatin1(GreekLocale), Qt::CaseInsensitive) == 0
        || languageTag(candidate) == QStringLiteral("el")) {
        return 2;
    }

    if (candidate.compare(QString::fromLatin1(EnglishLocale), Qt::CaseInsensitive) == 0
        || languageTag(candidate) == QStringLiteral("en")) {
        return 3;
    }

    return candidate.isEmpty() ? 5 : 4;
}

QString AnatomyVocabulary::localizedName(
    const AnatomicalSiteEntry &entry,
    const QString &requestedLocale) const
{
    const AnatomicalSiteLocalization *best = nullptr;
    int bestRank = 100;

    for (const auto &localization : entry.localizations) {
        if (localization.displayName.trimmed().isEmpty())
            continue;

        const auto rank = localeRank(localization.locale, requestedLocale);
        if (rank < bestRank) {
            best = &localization;
            bestRank = rank;
        }
    }

    return best == nullptr ? entry.code : best->displayName;
}

} // namespace GIPractice::Client::Clinical
