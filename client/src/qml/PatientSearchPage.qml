import QtQuick
import QtQuick.Controls as Controls
import QtQuick.Layouts
import org.kde.kirigami as Kirigami

Kirigami.Page {
    id: root

    required property var patientModel

    signal patientActivated(string patientId)

    title: i18n("Ασθενείς")

    Component.onCompleted: patientModel.search()

    ColumnLayout {
        anchors.fill: parent
        anchors.margins: Kirigami.Units.largeSpacing
        spacing: Kirigami.Units.largeSpacing

        Kirigami.Heading {
            Layout.fillWidth: true
            level: 2
            text: i18n("Αναζήτηση ασθενών")
        }

        Controls.Frame {
            Layout.fillWidth: true

            GridLayout {
                anchors.fill: parent
                columns: 3
                columnSpacing: Kirigami.Units.largeSpacing
                rowSpacing: Kirigami.Units.smallSpacing

                ColumnLayout {
                    Layout.fillWidth: true

                    Controls.Label {
                        text: i18n("Επώνυμο")
                    }

                    Controls.TextField {
                        Layout.fillWidth: true
                        text: root.patientModel.lastName
                        selectByMouse: true
                        onTextEdited: root.patientModel.lastName = text
                        onAccepted: root.patientModel.search()
                    }
                }

                ColumnLayout {
                    Layout.fillWidth: true

                    Controls.Label {
                        text: i18n("Όνομα")
                    }

                    Controls.TextField {
                        Layout.fillWidth: true
                        text: root.patientModel.firstName
                        selectByMouse: true
                        onTextEdited: root.patientModel.firstName = text
                        onAccepted: root.patientModel.search()
                    }
                }

                ColumnLayout {
                    Layout.fillWidth: true

                    Controls.Label {
                        text: i18n("Πατρώνυμο")
                    }

                    Controls.TextField {
                        Layout.fillWidth: true
                        text: root.patientModel.fathersName
                        selectByMouse: true
                        onTextEdited: root.patientModel.fathersName = text
                        onAccepted: root.patientModel.search()
                    }
                }

                ColumnLayout {
                    Layout.fillWidth: true

                    Controls.Label {
                        text: i18n("Ημερομηνία γέννησης από")
                    }

                    Controls.TextField {
                        Layout.fillWidth: true
                        text: root.patientModel.birthDateFrom
                        placeholderText: "YYYY-MM-DD"
                        inputMethodHints: Qt.ImhDate
                        selectByMouse: true
                        onTextEdited: root.patientModel.birthDateFrom = text
                        onAccepted: root.patientModel.search()
                    }
                }

                ColumnLayout {
                    Layout.fillWidth: true

                    Controls.Label {
                        text: i18n("Ημερομηνία γέννησης έως")
                    }

                    Controls.TextField {
                        Layout.fillWidth: true
                        text: root.patientModel.birthDateTo
                        placeholderText: "YYYY-MM-DD"
                        inputMethodHints: Qt.ImhDate
                        selectByMouse: true
                        onTextEdited: root.patientModel.birthDateTo = text
                        onAccepted: root.patientModel.search()
                    }
                }

                RowLayout {
                    Layout.fillWidth: true
                    Layout.alignment: Qt.AlignBottom

                    Controls.Button {
                        Layout.fillWidth: true
                        text: i18n("Καθαρισμός")
                        enabled: !root.patientModel.loading
                        onClicked: root.patientModel.clearCriteria()
                    }

                    Controls.Button {
                        Layout.fillWidth: true
                        text: i18n("Αναζήτηση")
                        enabled: !root.patientModel.loading
                        onClicked: root.patientModel.search()
                    }
                }
            }
        }

        Controls.Label {
            Layout.fillWidth: true
            visible: root.patientModel.errorMessage.length > 0
            text: root.patientModel.errorMessage
            color: Kirigami.Theme.negativeTextColor
            wrapMode: Text.WordWrap
        }

        RowLayout {
            Layout.fillWidth: true

            Kirigami.Heading {
                Layout.fillWidth: true
                level: 3
                text: i18n("Αποτελέσματα")
            }

            Controls.BusyIndicator {
                running: root.patientModel.loading
                visible: running
            }

            Controls.Label {
                visible: !root.patientModel.loading && root.patientModel.errorMessage.length === 0
                text: i18n("%1 ασθενείς", root.patientModel.totalCount)
            }
        }

        Controls.Frame {
            Layout.fillWidth: true
            Layout.fillHeight: true
            visible: root.patientModel.count > 0

            ListView {
                id: resultsList
                anchors.fill: parent
                clip: true
                model: root.patientModel
                currentIndex: -1

                delegate: Controls.ItemDelegate {
                    required property int index
                    required property string displayName
                    required property string fathersName
                    required property string birthDate

                    width: ListView.view.width
                    highlighted: ListView.isCurrentItem

                    contentItem: RowLayout {
                        spacing: Kirigami.Units.largeSpacing

                        Controls.Label {
                            Layout.fillWidth: true
                            text: displayName
                            font.bold: true
                            elide: Text.ElideRight
                        }

                        Controls.Label {
                            Layout.preferredWidth: Kirigami.Units.gridUnit * 14
                            text: fathersName.length > 0
                                  ? i18n("Πατρώνυμο: %1", fathersName)
                                  : i18n("Πατρώνυμο: —")
                            elide: Text.ElideRight
                        }

                        Controls.Label {
                            Layout.preferredWidth: Kirigami.Units.gridUnit * 10
                            text: birthDate.length > 0 ? birthDate : "—"
                            horizontalAlignment: Text.AlignRight
                        }
                    }

                    onClicked: {
                        resultsList.currentIndex = index
                        const patientId = root.patientModel.patientIdAt(index)
                        if (patientId.length > 0)
                            root.patientActivated(patientId)
                    }
                }

                Controls.ScrollBar.vertical: Controls.ScrollBar {}
            }
        }

        Controls.Label {
            Layout.fillWidth: true
            Layout.fillHeight: true
            visible: !root.patientModel.loading
                     && root.patientModel.count === 0
                     && root.patientModel.errorMessage.length === 0
            text: i18n("Δεν βρέθηκαν ασθενείς.")
            horizontalAlignment: Text.AlignHCenter
            verticalAlignment: Text.AlignVCenter
        }

        RowLayout {
            Layout.fillWidth: true
            visible: root.patientModel.totalCount > 0

            Controls.Button {
                text: i18n("Προηγούμενη")
                enabled: !root.patientModel.loading && root.patientModel.canGoPrevious
                onClicked: root.patientModel.previousPage()
            }

            Item {
                Layout.fillWidth: true
            }

            Controls.Label {
                text: i18n("Σελίδα %1", root.patientModel.page)
            }

            Item {
                Layout.fillWidth: true
            }

            Controls.Button {
                text: i18n("Επόμενη")
                enabled: !root.patientModel.loading && root.patientModel.canGoNext
                onClicked: root.patientModel.nextPage()
            }
        }
    }
}
