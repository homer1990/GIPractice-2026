import QtQuick
import QtQuick.Layouts
import org.kde.kirigami as Kirigami

Kirigami.ApplicationWindow {
    id: root

    width: 1100
    height: 720
    visible: true
    title: i18n("GIPractice")

    pageStack.initialPage: Kirigami.Page {
        title: i18n("GIPractice")

        ColumnLayout {
            anchors.centerIn: parent
            width: Math.min(parent.width - Kirigami.Units.gridUnit * 4,
                            Kirigami.Units.gridUnit * 34)
            spacing: Kirigami.Units.largeSpacing

            Kirigami.Heading {
                Layout.fillWidth: true
                level: 2
                text: i18n("Δοκιμή ανατομικού λεξιλογίου")
            }

            Kirigami.Label {
                Layout.fillWidth: true
                wrapMode: Text.WordWrap
                text: i18n("Δοκίμασε: άντρο, αντρο, corpus, GEJ, D2 ή sigmoid. Η επιλογή εμφανίζεται στα ελληνικά, ενώ ο εσωτερικός κωδικός παραμένει κρυφός.")
            }

            AnatomyAutocompleteField {
                Layout.fillWidth: true
                suggestionModel: anatomySuggestions
            }
        }
    }
}
