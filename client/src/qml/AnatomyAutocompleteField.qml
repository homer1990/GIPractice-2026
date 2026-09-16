import QtQuick
import QtQuick.Controls as Controls
import QtQuick.Layouts
import org.kde.kirigami as Kirigami

Item {
    id: root

    required property var suggestionModel

    property alias text: input.text
    property string sourceText: ""
    property string selectedCode: ""
    property string selectedDisplayName: ""
    property string placeholderText: i18n("Ανατομική θέση")

    signal accepted(string code, string displayName)

    implicitWidth: Kirigami.Units.gridUnit * 28
    implicitHeight: content.implicitHeight

    function clearSelection() {
        selectedCode = ""
        selectedDisplayName = ""
        sourceText = ""
        input.clear()
        suggestionModel.query = ""
        suggestionList.currentIndex = -1
        input.forceActiveFocus()
    }

    function acceptRow(row, displayName) {
        const code = suggestionModel.codeAt(row)
        if (!code)
            return

        sourceText = input.text
        selectedCode = code
        selectedDisplayName = displayName
        suggestionModel.query = ""
        suggestionList.currentIndex = -1
        accepted(code, displayName)
        input.forceActiveFocus()
    }

    ColumnLayout {
        id: content
        anchors.left: parent.left
        anchors.right: parent.right
        spacing: Kirigami.Units.smallSpacing

        Controls.TextField {
            id: input
            Layout.fillWidth: true
            placeholderText: root.placeholderText
            selectByMouse: true

            onTextEdited: {
                root.sourceText = text

                if (root.selectedCode) {
                    root.selectedCode = ""
                    root.selectedDisplayName = ""
                }

                root.suggestionModel.query = text
                suggestionList.currentIndex = root.suggestionModel.count > 0 ? 0 : -1
            }

            Keys.onDownPressed: event => {
                if (root.suggestionModel.count <= 0)
                    return

                suggestionList.currentIndex = Math.min(
                    suggestionList.currentIndex + 1,
                    root.suggestionModel.count - 1)
                event.accepted = true
            }

            Keys.onUpPressed: event => {
                if (root.suggestionModel.count <= 0)
                    return

                suggestionList.currentIndex = Math.max(suggestionList.currentIndex - 1, 0)
                event.accepted = true
            }

            Keys.onReturnPressed: event => {
                if (suggestionList.currentIndex >= 0 && root.suggestionModel.count > 0) {
                    const item = suggestionList.itemAtIndex(suggestionList.currentIndex)
                    if (item)
                        root.acceptRow(suggestionList.currentIndex, item.displayName)
                    event.accepted = true
                }
            }

            Keys.onEnterPressed: event => {
                if (suggestionList.currentIndex >= 0 && root.suggestionModel.count > 0) {
                    const item = suggestionList.itemAtIndex(suggestionList.currentIndex)
                    if (item)
                        root.acceptRow(suggestionList.currentIndex, item.displayName)
                    event.accepted = true
                }
            }

            Keys.onEscapePressed: event => {
                root.suggestionModel.query = ""
                suggestionList.currentIndex = -1
                event.accepted = true
            }
        }

        Controls.Frame {
            Layout.fillWidth: true
            visible: root.suggestionModel.query.trim().length > 0
                     && root.suggestionModel.count > 0
            Layout.preferredHeight: visible
                                    ? Math.min(suggestionList.contentHeight
                                               + topPadding + bottomPadding,
                                               Kirigami.Units.gridUnit * 14)
                                    : 0

            ListView {
                id: suggestionList
                anchors.fill: parent
                clip: true
                model: root.suggestionModel
                currentIndex: model.count > 0 ? 0 : -1

                delegate: Controls.ItemDelegate {
                    required property int index
                    required property string displayName
                    required property string matchedText
                    required property string matchedLocale
                    required property bool exactMatch

                    width: ListView.view.width
                    highlighted: ListView.isCurrentItem

                    contentItem: Column {
                        spacing: 0

                        Controls.Label {
                            width: parent.width
                            text: displayName
                            elide: Text.ElideRight
                        }

                        Controls.Label {
                            width: parent.width
                            visible: matchedText.length > 0 && matchedText !== displayName
                            text: i18nc("anatomy autocomplete matched alias", "Αντιστοίχιση: %1", matchedText)
                            opacity: 0.65
                            font.pointSize: Kirigami.Theme.smallFont.pointSize
                            elide: Text.ElideRight
                        }
                    }

                    onClicked: root.acceptRow(index, displayName)
                }

                Controls.ScrollBar.vertical: Controls.ScrollBar {}
            }
        }

        Controls.Frame {
            Layout.fillWidth: true
            visible: root.selectedCode.length > 0

            RowLayout {
                anchors.fill: parent

                Controls.Label {
                    Layout.fillWidth: true
                    text: root.selectedDisplayName
                    font.bold: true
                    elide: Text.ElideRight
                }

                Controls.ToolButton {
                    text: "×"
                    Accessible.name: i18n("Αφαίρεση ανατομικής θέσης")
                    onClicked: root.clearSelection()
                }
            }
        }
    }
}
