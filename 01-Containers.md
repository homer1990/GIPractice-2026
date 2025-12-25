flowchart TB
  Secretary["Secretary"] --> WPF["WPF Client\nGIPractice.Wpf"]
  Doctor["Doctor"] --> WPF
  Admin["Admin"] --> WPF

  Pathologist["Pathologist/Lab"] --> Portal["Lab Portal (Web)\n(optional but recommended)"]

  WPF --> Client["Client SDK\nGIPractice.Client"]
  Portal --> API["GIPractice API\nGIPractice.Api"]
  Client --> API

  API --> DB[(MariaDB/MySQL)]
  API --> Files[(File Storage\nreports/images/videos)]
  API --> Notify["Notifications Engine\nrecalls/open-reschedule reminders"]

  WPF --> CalSync["Calendar Sync Service\n(Google/M365)"]
  CalSync --> CloudCal["Cloud Calendar Provider"]
