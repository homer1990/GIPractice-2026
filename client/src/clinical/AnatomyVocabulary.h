#pragma once

#include <QLocale>
#include <QList>
#include <QString>
#include <QStringList>

#include <optional>

namespace GIPractice::Client::Clinical {

enum class AnatomicalSiteKind
{
    Organ,
    Region,
    Landmark,
    Other
};

struct AnatomicalSiteLocalization
{
    QString locale;
    QString displayName;
    QStringList aliases;
};

struct AnatomicalSiteEntry
{
    QString code;
    AnatomicalSiteKind kind = AnatomicalSiteKind::Other;
    QString parentCode;
    int sortOrder = 0;
    QList<AnatomicalSiteLocalization> localizations;
};

struct AnatomicalSiteSuggestion
{
    QString code;
    QString displayName;
    QString matchedText;
    QString matchedLocale;
    AnatomicalSiteKind kind = AnatomicalSiteKind::Other;
    bool exactMatch = false;
};

// Client-side view over the controlled anatomy vocabulary.
//
// Canonical codes are language-neutral. Names and aliases are localized input/display
// metadata only. The class deliberately has no networking or persistence dependency:
// API/SQLite code can populate it later without changing the lookup rules used by QML.
class AnatomyVocabulary final
{
public:
    static constexpr auto GreekLocale = "el-GR";
    static constexpr auto EnglishLocale = "en";

    AnatomyVocabulary() = default;
    explicit AnatomyVocabulary(QList<AnatomicalSiteEntry> entries);

    void setEntries(QList<AnatomicalSiteEntry> entries);
    [[nodiscard]] const QList<AnatomicalSiteEntry> &entries() const noexcept;

    [[nodiscard]] QString displayName(
        const QString &code,
        const QLocale &requestedLocale = QLocale::system()) const;

    [[nodiscard]] std::optional<AnatomicalSiteSuggestion> resolveExact(
        const QString &text,
        const QLocale &requestedLocale = QLocale::system()) const;

    [[nodiscard]] QList<AnatomicalSiteSuggestion> suggest(
        const QString &text,
        const QLocale &requestedLocale = QLocale::system(),
        int limit = 8) const;

    [[nodiscard]] QList<AnatomicalSiteEntry> childrenOf(const QString &parentCode) const;

private:
    QList<AnatomicalSiteEntry> m_entries;

    [[nodiscard]] const AnatomicalSiteEntry *findByCode(const QString &code) const;
    [[nodiscard]] static QString normalizeForMatch(const QString &text);
    [[nodiscard]] static QString localeTag(const QLocale &locale);
    [[nodiscard]] static QString languageTag(const QString &locale);
    [[nodiscard]] static int localeRank(const QString &candidateLocale, const QString &requestedLocale);
    [[nodiscard]] QString localizedName(
        const AnatomicalSiteEntry &entry,
        const QString &requestedLocale) const;
};

} // namespace GIPractice::Client::Clinical
