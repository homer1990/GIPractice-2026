import QtQuick
import QtQuick.Layouts
import org.kde.kirigami as Kirigami

Kirigami.ApplicationWindow {
    id: root

    width: 1440
    height: 900
    minimumWidth: 960
    minimumHeight: 640
    visible: true
    title: i18n("GIPractice — %1", currentSection)

    property string currentSection: i18n("Home")
    property string currentIcon: "go-home-symbolic"

    function openSection(title, iconName) {
        currentSection = title
        currentIcon = iconName
    }

    globalDrawer: Kirigami.GlobalDrawer {
        modal: false
        collapsible: true
        title: i18n("GIPractice")
        titleIcon: "medical-symbolic"

        actions: [
            Kirigami.Action {
                text: i18n("Home")
                icon.name: "go-home-symbolic"
                onTriggered: root.openSection(text, icon.name)
            },
            Kirigami.Action {
                text: i18n("Schedule")
                icon.name: "view-calendar-symbolic"
                onTriggered: root.openSection(text, icon.name)
            },
            Kirigami.Action {
                text: i18n("Patients")
                icon.name: "system-users-symbolic"
                onTriggered: root.openSection(text, icon.name)
            },
            Kirigami.Action {
                text: i18n("Endoscopies")
                icon.name: "document-edit-symbolic"
                onTriggered: root.openSection(text, icon.name)
            },
            Kirigami.Action {
                text: i18n("Practice")
                icon.name: "folder-symbolic"
                onTriggered: root.openSection(text, icon.name)
            },
            Kirigami.Action {
                text: i18n("Settings")
                icon.name: "settings-configure-symbolic"
                onTriggered: root.openSection(text, icon.name)
            }
        ]
    }

    pageStack.initialPage: Kirigami.Page {
        title: root.currentSection

        ColumnLayout {
            anchors.centerIn: parent
            spacing: Kirigami.Units.largeSpacing

            Kirigami.Icon {
                Layout.alignment: Qt.AlignHCenter
                implicitWidth: Kirigami.Units.iconSizes.huge
                implicitHeight: implicitWidth
                source: root.currentIcon
            }

            Kirigami.Heading {
                Layout.alignment: Qt.AlignHCenter
                level: 1
                text: root.currentSection
            }

            Kirigami.Label {
                Layout.alignment: Qt.AlignHCenter
                text: i18n("Module shell ready. Functional UI will be added one workflow at a time.")
                opacity: 0.7
            }
        }
    }
}
