classDiagram
  %% PEOPLE
  class Patient {
    +Id
    +FirstName
    +LastName
    +Phone
    +Email?
    +BirthDate?
    +Gender?
    +Address?
  }

  %% PLANNING TRUTH
  class Appointment {
    +Id
    +PatientId
    +ScheduledStart
    +ExpectedDurationMin
    +AppointmentTypeId
    +Status
    +Notes?
  }

  class AppointmentType {
    +Id
    +Name
    +MeanDurationMin
    +Defaults/Rules
  }

  %% REALITY TRUTH
  class Encounter {
    +Id
    +PatientId
    +AppointmentId?
    +Kind  // office, clinical, endo, recto, INFAI, exam/ortho...
    +ActualStart
    +ActualEnd?
    +Notes?
  }

  %% ENDOSCOPY SESSION (Gastro/Colono/Double)
  class EndoscopySession {
    +Id
    +EncounterId
    +SessionType  // Gastro/Colono/Double
    +IsUrgent
    +Notes?
    +ComputedReportRef?
  }

  class MediaAsset {
    +Id
    +EndoscopySessionId
    +Type  // photo/video
    +StorageRef
    +CapturedAt
  }

  class BiopsyBottle {
    +Id
    +EndoscopySessionId
    +Label
    +Site?
    +PrintedAt?
  }

  %% DISPATCH (weekly protocol)
  class DispatchProtocol {
    +Id
    +ProtocolNo
    +CreatedAt
    +Courier?
    +TrackingNo?
    +SentAt?
    +DeliveredAt?
    +Status  // Open/Sent/Delivered/Closed
  }

  class DispatchItem {
    +Id
    +DispatchProtocolId
    +EndoscopySessionId

    %% SNAPSHOT fields (Excel columns)
    +PatientFirstNameSnapshot
    +PatientLastNameSnapshot
    +SessionTypeSnapshot  // Gastro/Colono/Double
    +CalculatedPriceSnapshot
    +TubeCountSnapshot
    +IsUrgentSnapshot

    +LabStatus  // Pending/InLab/ReportReady/Closed
  }

  class PathologyReport {
    +Id
    +DispatchItemId
    +ReceivedAt?
    +ReportDocumentRef?
    +Summary?
    +ParcelId?
  }

  %% INFAI (report comes from company)
  class INFAI {
    +Id
    +EncounterId
    +IsUrgent
    +Status  // Planned/SampleTaken/Sent/ReportReceived/Closed
    +CompanyReportRef?
    +ReportReceivedAt?
  }

  %% TASKS/QUEUES
  class RescheduleTask {
    +Id
    +AppointmentId
    +ContactOn?
    +Note?
    +Status
  }

  class RecallTask {
    +Id
    +PatientId
    +DueOn
    +RecommendedKind
    +Reason
    +Status
  }

  class DayMeta {
    +Date
    +IsHoliday
    +IsDayOff
    +Note?
  }

  %% RELATIONS
  Patient "1" --> "0..*" Appointment
  Patient "1" --> "0..*" Encounter
  AppointmentType "1" --> "0..*" Appointment

  Appointment "0..1" --> "0..1" Encounter : resolvedInto
  Appointment "0..1" --> "0..1" RescheduleTask : openReschedule

  Encounter "1" --> "0..1" EndoscopySession
  EndoscopySession "1" --> "0..*" MediaAsset
  EndoscopySession "1" --> "0..*" BiopsyBottle

  DispatchProtocol "1" --> "0..*" DispatchItem
  DispatchItem "1" --> "1" EndoscopySession
  DispatchItem "1" --> "0..1" PathologyReport

  Encounter "1" --> "0..1" INFAI

  Patient "1" --> "0..*" RecallTask
  DayMeta "1" --> "1" ClinicDay : constraints
