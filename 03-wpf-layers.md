flowchart TB
  subgraph WPF["GIPractice.Wpf"]
    Views["Views\nPages + UserControls + Templates"]
    VMs["ViewModels\nSchedulerVMs + Search* + Details*"]
    UIInfra["UI Infra\nNavigation + Overlays/Dialogs + Busy/Error"]
    Resources["Resources\nStyling + Localization"]
  end

  subgraph Services["WPF Services"]
    Nav["NavigationService"]
    Dialogs["Overlay/DialogService"]
    Theme["Theme/StylingService"]
    I18N["LocalizationService\nCulture/Translation"]
    Capture["CaptureService\nUSB stick abstraction"]
    Print["PrintService\nlabels + dispatch protocol"]
    CalendarSync["CalendarSyncService\nGoogle/M365"]
  end

  subgraph Client["GIPractice.Client modules"]
    Patients["PatientsModule"]
    Appointments["AppointmentsModule"]
    Encounters["EncountersModule"]
    Endoscopies["EndoscopiesModule"]
    Biopsies["BiopsyModule"]
    Dispatch["DispatchModule"]
    Pathology["PathologyReportsModule"]
    INFAI["INFAIModule"]
    Tasks["TasksModule\n(reschedule + recall)"]
    DayMeta["DayMetaModule"]
    SettingsM["SettingsModule"]
  end

  Views --> VMs
  VMs --> UIInfra
  VMs --> Nav
  VMs --> Dialogs
  VMs --> Theme
  VMs --> I18N
  VMs --> Capture
  VMs --> Print
  VMs --> CalendarSync

  VMs --> Patients
  VMs --> Appointments
  VMs --> Encounters
  VMs --> Endoscopies
  VMs --> Biopsies
  VMs --> Dispatch
  VMs --> Pathology
  VMs --> INFAI
  VMs --> Tasks
  VMs --> DayMeta
  VMs --> SettingsM

  Resources --> Theme
  Resources --> I18N
