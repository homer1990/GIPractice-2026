sequenceDiagram
  actor Sec as Secretary
  participant Day as SchedulerDayViewModel
  participant Appt as Appointment
  participant Enc as Encounter

  Sec->>Day: Resolve(appointment)
  Day->>Appt: Status = InProgress
  Day->>Enc: Create (ActualStart=now, AppointmentId)
  Day->>Day: Show Active Encounter panel

  Sec->>Day: END (now or manual time)
  Day->>Enc: Set ActualEnd
  Day->>Appt: Status = Completed
  Day->>Day: Clear Active Encounter panel

sequenceDiagram
  actor Sec as Secretary
  participant Day as SchedulerDayViewModel
  participant Appt as Appointment
  participant Task as RescheduleTask

  Sec->>Day: OpenReschedule(appointment)
  Day->>Appt: Status = NeedsReschedule
  Day->>Task: Create(ContactOn, Note)
  Day->>Day: Remove from day plan; add to open-reschedule queue

sequenceDiagram
  actor Sec as Secretary
  participant UI as DispatchProtocolDetailsViewModel
  participant DP as DispatchProtocol
  participant Item as DispatchItem
  participant Sess as EndoscopySession

  Sec->>UI: Create protocol (weekly)
  UI->>DP: Create(Open)

  Sec->>UI: Add EndoscopySessions to protocol
  loop for each session
    UI->>Sess: Read session + patient + bottles count
    UI->>Item: Create with SNAPSHOTS (name/type/price/tubes/urgent)
  end

  Sec->>UI: Print protocol + labels
  Sec->>UI: Mark Sent (tracking)
  UI->>DP: SentAt + TrackingNo + Status=Sent

sequenceDiagram
  actor Lab as Pathologist
  participant Portal as Lab Portal
  participant Item as DispatchItem
  participant Report as PathologyReport

  Lab->>Portal: Open protocol items
  Lab->>Portal: Upload report for item
  Portal->>Item: LabStatus=ReportReady
  Portal->>Report: Create/Update (ReceivedAt + doc ref + summary)

sequenceDiagram
  participant WPF as WPF Scheduler
  participant Sync as CalendarSyncService
  participant Cloud as Cloud Calendar
  participant Plan as Appointments (planning truth)

  WPF->>Plan: list day/week appointments
  WPF->>Sync: push changes
  Sync->>Cloud: create/update/delete events (mapped from appointments)
  Cloud-->>Sync: success/conflicts
  Sync-->>WPF: status + conflict list
