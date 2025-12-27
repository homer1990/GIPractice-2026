namespace GIPractice.Contracts.Infai;

public enum InfaiReportStatus
{
    Draft = 0,              // created, not yet sent / not yet committed
    SamplesReady = 1,       // bottles collected & ready to ship (or “we have them”)
    SentToCompany = 2,      // shipped/handed over
    ReceivedFromCompany = 3,// report came back (file or structured data)
    Reviewed = 4,           // doctor reviewed/approved internally
    PatientInformed = 5,    // patient informed of result
    Closed = 6,             // fully done (optional; sometimes PatientInformed is enough)
    Cancelled = 7           // aborted (lost sample, patient withdrew, etc.)
}
