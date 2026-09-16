#include "clinical/AnatomySuggestionModel.h"

#include <KAboutData>
#include <KLocalization>
#include <KLocalizedString>

#include <QApplication>
#include <QQmlApplicationEngine>
#include <QQuickStyle>
#include <QtQml/qqml.h>

using GIPractice::Client::Clinical::AnatomySuggestionModel;

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

    QQmlApplicationEngine engine;
    KLocalization::setupLocalizedContext(&engine);
    engine.loadFromModule(QStringLiteral("net.gmanthos.gipractice"), QStringLiteral("Main"));

    if (engine.rootObjects().isEmpty())
        return 1;

    return app.exec();
}
