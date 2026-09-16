#include "api/PracticeApiClient.h"
#include "clinical/AnatomySuggestionModel.h"

#include <KAboutData>
#include <KLocalizedQmlContext>
#include <KLocalizedString>

#include <QApplication>
#include <QDebug>
#include <QQmlApplicationEngine>
#include <QQmlContext>
#include <QQuickStyle>
#include <QtQml/qqml.h>

#include <variant>

using GIPractice::Client::Api::ApiError;
using GIPractice::Client::Api::PatientSearchRequest;
using GIPractice::Client::Api::PatientSearchResponse;
using GIPractice::Client::Api::PracticeApiClient;
using GIPractice::Client::Clinical::AnatomicalSiteEntry;
using GIPractice::Client::Clinical::AnatomicalSiteKind;
using GIPractice::Client::Clinical::AnatomicalSiteLocalization;
using GIPractice::Client::Clinical::AnatomySuggestionModel;

namespace {

// Temporary smoke-test vocabulary for the first client control. The production client
// will receive the full localized vocabulary from the server/database; keeping this here
// only makes the autocomplete behavior testable before that transport exists.
QList<AnatomicalSiteEntry> developmentAnatomyEntries()
{
    return {
        AnatomicalSiteEntry{
            QStringLiteral("STOMACH"),
            AnatomicalSiteKind::Organ,
            QString{},
            10,
            QList<AnatomicalSiteLocalization>{
                AnatomicalSiteLocalization{
                    QStringLiteral("el-GR"),
                    QStringLiteral("Στόμαχος"),
                    QStringList{QStringLiteral("στομάχι"), QStringLiteral("στομαχος"), QStringLiteral("gastric")}},
                AnatomicalSiteLocalization{
                    QStringLiteral("en"),
                    QStringLiteral("Stomach"),
                    QStringList{QStringLiteral("gastric")}}
            }},
        AnatomicalSiteEntry{
            QStringLiteral("STOMACH_ANTRUM"),
            AnatomicalSiteKind::Region,
            QStringLiteral("STOMACH"),
            20,
            QList<AnatomicalSiteLocalization>{
                AnatomicalSiteLocalization{
                    QStringLiteral("el-GR"),
                    QStringLiteral("Άντρο στομάχου"),
                    QStringList{QStringLiteral("άντρο"), QStringLiteral("αντρο"), QStringLiteral("antrum"), QStringLiteral("antral")}},
                AnatomicalSiteLocalization{
                    QStringLiteral("en"),
                    QStringLiteral("Gastric antrum"),
                    QStringList{QStringLiteral("antrum"), QStringLiteral("antral")}}
            }},
        AnatomicalSiteEntry{
            QStringLiteral("STOMACH_CORPUS"),
            AnatomicalSiteKind::Region,
            QStringLiteral("STOMACH"),
            30,
            QList<AnatomicalSiteLocalization>{
                AnatomicalSiteLocalization{
                    QStringLiteral("el-GR"),
                    QStringLiteral("Σώμα στομάχου"),
                    QStringList{QStringLiteral("σώμα"), QStringLiteral("σωμα"), QStringLiteral("corpus"), QStringLiteral("body")}},
                AnatomicalSiteLocalization{
                    QStringLiteral("en"),
                    QStringLiteral("Gastric body"),
                    QStringList{QStringLiteral("body"), QStringLiteral("corpus"), QStringLiteral("stomach body")}}
            }},
        AnatomicalSiteEntry{
            QStringLiteral("GEJ"),
            AnatomicalSiteKind::Landmark,
            QStringLiteral("ESOPHAGUS"),
            40,
            QList<AnatomicalSiteLocalization>{
                AnatomicalSiteLocalization{
                    QStringLiteral("el-GR"),
                    QStringLiteral("Γαστροοισοφαγική συμβολή"),
                    QStringList{QStringLiteral("ΓΟΣ"), QStringLiteral("GEJ"), QStringLiteral("γαστροοισοφαγικη συμβολη")}},
                AnatomicalSiteLocalization{
                    QStringLiteral("en"),
                    QStringLiteral("Gastroesophageal junction"),
                    QStringList{QStringLiteral("GEJ"), QStringLiteral("gastro-esophageal junction")}}
            }},
        AnatomicalSiteEntry{
            QStringLiteral("DIAPHRAGMATIC_IMPRESSION"),
            AnatomicalSiteKind::Landmark,
            QStringLiteral("ESOPHAGUS"),
            50,
            QList<AnatomicalSiteLocalization>{
                AnatomicalSiteLocalization{
                    QStringLiteral("el-GR"),
                    QStringLiteral("Διαφραγματικό εντύπωμα"),
                    QStringList{QStringLiteral("διαφραγματική εντύπωση"), QStringLiteral("diaphragmatic pinch")}},
                AnatomicalSiteLocalization{
                    QStringLiteral("en"),
                    QStringLiteral("Diaphragmatic impression"),
                    QStringList{QStringLiteral("diaphragmatic depression"), QStringLiteral("diaphragmatic pinch")}}
            }},
        AnatomicalSiteEntry{
            QStringLiteral("DUODENUM_D2"),
            AnatomicalSiteKind::Region,
            QStringLiteral("DUODENUM"),
            60,
            QList<AnatomicalSiteLocalization>{
                AnatomicalSiteLocalization{
                    QStringLiteral("el-GR"),
                    QStringLiteral("2η μοίρα δωδεκαδακτύλου"),
                    QStringList{QStringLiteral("2η μοίρα"), QStringLiteral("2η μοιρα"), QStringLiteral("D2"), QStringLiteral("d2")}},
                AnatomicalSiteLocalization{
                    QStringLiteral("en"),
                    QStringLiteral("Second duodenal portion"),
                    QStringList{QStringLiteral("D2"), QStringLiteral("second part duodenum")}}
            }},
        AnatomicalSiteEntry{
            QStringLiteral("SIGMOID_COLON"),
            AnatomicalSiteKind::Region,
            QStringLiteral("COLON"),
            70,
            QList<AnatomicalSiteLocalization>{
                AnatomicalSiteLocalization{
                    QStringLiteral("el-GR"),
                    QStringLiteral("Σιγμοειδές κόλον"),
                    QStringList{QStringLiteral("σιγμοειδές"), QStringLiteral("σιγμοειδες"), QStringLiteral("sigmoid")}},
                AnatomicalSiteLocalization{
                    QStringLiteral("en"),
                    QStringLiteral("Sigmoid colon"),
                    QStringList{QStringLiteral("sigmoid")}}
            }}
    };
}

void startApiSmokeTest(QApplication &app)
{
    const auto apiUrlText = qEnvironmentVariable("GIPRACTICE_API_URL").trimmed();
    if (apiUrlText.isEmpty())
        return;

    auto *api = new PracticeApiClient(QUrl::fromUserInput(apiUrlText), &app);
    api->searchPatients(PatientSearchRequest{}, [](auto result) {
        if (const auto *response = std::get_if<PatientSearchResponse>(&result)) {
            qInfo() << "GIPractice API connected; patient count:" << response->totalCount;
            return;
        }

        const auto &error = std::get<ApiError>(result);
        qWarning() << "GIPractice API patient search failed:"
                   << error.httpStatus
                   << error.message;
    });
}

} // namespace

int main(int argc, char *argv[])
{
    if (qEnvironmentVariableIsEmpty("QT_QUICK_CONTROLS_STYLE"))
        QQuickStyle::setStyle(QStringLiteral("org.kde.desktop"));

    QApplication app(argc, argv);

    KLocalizedString::setApplicationDomain("gipractice");

    KAboutData aboutData(
        QStringLiteral("gipractice"),
        i18nc("@title", "GIPractice"),
        QStringLiteral("0.1.0"),
        i18nc("@info", "Endoscopy practice management"),
        KAboutLicense::GPL_V3,
        QStringLiteral("Copyright 2026"));
    KAboutData::setApplicationData(aboutData);

    qmlRegisterType<AnatomySuggestionModel>(
        "net.gmanthos.gipractice",
        1,
        0,
        "AnatomySuggestionModel");

    AnatomySuggestionModel anatomySuggestions;
    anatomySuggestions.setEntries(developmentAnatomyEntries());

    startApiSmokeTest(app);

    QQmlApplicationEngine engine;

    auto *localizedContext = new KLocalizedQmlContext(&engine);
    localizedContext->setTranslationDomain(QStringLiteral("gipractice"));
    engine.rootContext()->setContextObject(localizedContext);
    engine.rootContext()->setContextProperty(QStringLiteral("anatomySuggestions"), &anatomySuggestions);

    engine.loadFromModule(QStringLiteral("net.gmanthos.gipractice"), QStringLiteral("Main"));

    if (engine.rootObjects().isEmpty())
        return 1;

    return app.exec();
}
