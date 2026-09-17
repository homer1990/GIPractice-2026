import QtQuick
import org.kde.kirigami as Kirigami

Kirigami.ApplicationWindow {
    id: root

    width: 1100
    height: 720
    visible: true
    title: i18n("GIPractice")

    pageStack.initialPage: PatientSearchPage {
        patientModel: patientSearchModel
    }
}
