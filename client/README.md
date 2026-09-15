# Qt/KDE client

The desktop client will use Qt 6, Qt Quick/QML, KDE Frameworks 6 and Kirigami.

The UI works with Patients, Appointments, Endoscopies, Exams, Prescriptions, Visits and INFAI.

Encounter is intentionally absent from the presentation model. A C++ service may keep an opaque internal clinical-session key while an editor/workflow is open, but QML must not expose Encounter as a screen, command, list item or editable object.

Client implementation starts after the first server write/read contracts are stable.
