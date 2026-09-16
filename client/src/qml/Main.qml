import QtQuick
import org.kde.kirigami as Kirigami

Kirigami.ApplicationWindow {
    id: root

    width: 1100
    height: 720
    visible: true
    title: i18n("GIPractice")

    pageStack.initialPage: Kirigami.Page {
        title: i18n("GIPractice")

        Kirigami.PlaceholderMessage {
            anchors.centerIn: parent
            width: Math.min(parent.width - Kirigami.Units.gridUnit * 4,
                            Kirigami.Units.gridUnit * 32)
            text: i18n("Client shell is running")
            explanation: i18n("Clinical screens will be added after the first server and persistence contracts are stable.")
        }
    }
}
