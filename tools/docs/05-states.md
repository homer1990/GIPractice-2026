stateDiagram-v2
  state "Appointment" as A {
    [*] --> Scheduled
    Scheduled --> Cancelled : cancel
    Scheduled --> NoShow : no-show
    Scheduled --> NeedsReschedule : open-reschedule
    NeedsReschedule --> Scheduled : rescheduled(new slot)
    Scheduled --> CheckedIn : check-in
    CheckedIn --> InProgress : resolve/start encounter
    InProgress --> Completed : end encounter
  }

  state "DispatchProtocol" as DP {
    [*] --> Open
    Open --> Sent : mark sent (tracking)
    Sent --> Delivered : mark delivered
    Delivered --> Closed
  }

  state "DispatchItem (LabStatus)" as DI {
    [*] --> Pending
    Pending --> InLab
    InLab --> ReportReady
    ReportReady --> Closed
  }

  state "INFAI" as I {
    [*] --> Planned
    Planned --> SampleTaken
    SampleTaken --> Sent
    Sent --> ReportReceived
    ReportReceived --> Closed
  }

  state "RescheduleTask" as R {
    [*] --> Open
    Open --> Contacted
    Contacted --> ScheduledAgain
    Contacted --> Closed
  }

  state "RecallTask" as T {
    [*] --> Pending
    Pending --> Notified
    Notified --> Scheduled
    Pending --> Snoozed
    Snoozed --> Pending
    Scheduled --> Closed
  }
