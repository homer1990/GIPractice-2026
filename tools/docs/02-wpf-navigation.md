flowchart TB
  Shell["Shell/MainWindow\nMenu + Navigation + Global Resources"] --> Scheduler["Scheduler (Main)"]
  Shell --> Searches["Search Hub (or menu entries)"]
  Shell --> Settings["Settings"]
  Shell --> AdminUsers["Users/Roles (optional)"]

  %% SEARCH + DETAILS (every main object gets both)
  Searches --> S_Pat["Patients Search"]
  Searches --> S_Appt["Appointments Search"]
  Searches --> S_Enc["Encounters/Visits Search"]
  Searches --> S_Endo["Endoscopy Sessions Search"]
  Searches --> S_Bottle["Biopsy Bottles Search"]
  Searches --> S_Dispatch["Dispatch Protocols Search"]
  Searches --> S_DispatchItem["Dispatch Items Search"]
  Searches --> S_Path["Pathology Reports Search"]
  Searches --> S_Exam["Exams/Orthos (HEINE) Search"]
  Searches --> S_INFAI["INFAI Search"]
  Searches --> S_Recall["Recall/Notification Tasks Search"]
  Searches --> S_ResTask["Open Reschedule Tasks Search"]

  S_Pat --> D_Pat["Patient Details Editor"]
  S_Appt --> D_Appt["Appointment Details Editor"]
  S_Enc --> D_Enc["Encounter/Visit Details Editor"]
  S_Endo --> D_Endo["Endoscopy Editor\nCapture + Bottles + Report"]
  S_Bottle --> D_Bottle["Biopsy Bottle Details\n(+ print label)"]
  S_Dispatch --> D_Dispatch["Dispatch Protocol Details\n(items + print + mark sent)"]
  S_DispatchItem --> D_DispatchItem["Dispatch Item Details\n(snapshot + lab status)"]
  S_Path --> D_Path["Pathology Report Details\n(upload/view)"]
  S_Exam --> D_Exam["Exam/Ortho Details"]
  S_INFAI --> D_INFAI["INFAI Details\n(sample + report upload)"]
  S_Recall --> D_Recall["Recall Task Details\n(due date + recommended kind)"]
  S_ResTask --> D_ResTask["Reschedule Task Details\n(contactOn + note)"]

  %% SCHEDULER
  Scheduler --> Day["Day Scheduler View"]
  Day --> Planned["Planned appointments list\n(reschedule/open-reschedule/cancel/resolve)"]
  Day --> Active["Active Encounter panel\n(END / DESELECT)"]
  Day --> RecallQueue["Recall Queue\n(patients due / findings warrant)"]
  Day --> OpenResQueue["Open Reschedule Queue\n(contact later)"]
  Day --> DayMeta["Day Meta\nHoliday/DayOff/Note"]
  Day --> CalStatus["Cloud Calendar Sync Status"]
  Day --> D_Endo
  Day --> D_Enc

  %% ENDOSCOPY
  D_Endo --> Capture["USB Capture Stick\nphoto/video"]
  D_Endo --> Labels["Bottle labels printing"]
  D_Endo --> ComputedReport["Computed Endoscopy Report\n(FindingsOrgansEndoscopies + notes + diagnoses)"]
