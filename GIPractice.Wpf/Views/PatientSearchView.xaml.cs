using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GIPractice.Wpf.Views
{
    /// <summary>
    /// Interaction logic for PatientSearchPanel.xaml
    /// </summary>
    public partial class PatientSearchView : UserControl
    {
        public PatientSearchView()
        {
            InitializeComponent();
            PatientsDataGrid.ItemsSource = MockPatients.Create();
        }
    }

    public sealed class PatientGridRow
    {
        public string PersonalNumber { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string FatherName { get; set; } = "";
        public DateTime Birthdate { get; set; }
        public DateTime? LastVisitDate { get; set; }
        public DateTime? LastEndoscopyDate { get; set; }
    }

    public static class MockPatients
    {
        public static List<PatientGridRow> Create() =>
        [
            new() { PersonalNumber="PN-000001", FirstName="Γεώργιος", LastName="Παπαδόπουλος", FatherName="Δημήτριος", Birthdate=new DateTime(1978, 3, 12), LastVisitDate=new DateTime(2025, 11, 8),  LastEndoscopyDate=new DateTime(2025, 10, 21) },
            new() { PersonalNumber="PN-000002", FirstName="Μαρία",     LastName="Νικολάου",    FatherName="Ιωάννης",  Birthdate=new DateTime(1986, 7, 2),  LastVisitDate=new DateTime(2025, 9, 14),  LastEndoscopyDate=new DateTime(2024, 12, 5) },
            new() { PersonalNumber="PN-000003", FirstName="Κωνσταντίνος", LastName="Αντωνίου", FatherName="Παναγιώτης", Birthdate=new DateTime(1969, 1, 29), LastVisitDate=new DateTime(2025, 6, 30), LastEndoscopyDate=new DateTime(2025, 6, 30) },
            new() { PersonalNumber="PN-000004", FirstName="Ελένη",     LastName="Γεωργίου",   FatherName="Στέλιος",  Birthdate=new DateTime(1992, 11, 18), LastVisitDate=new DateTime(2025, 8, 3),  LastEndoscopyDate=null },
            new() { PersonalNumber="PN-000005", FirstName="Νικόλαος",  LastName="Σταματίου",  FatherName="Αλέξανδρος", Birthdate=new DateTime(1958, 5, 9),  LastVisitDate=new DateTime(2025, 12, 1), LastEndoscopyDate=new DateTime(2023, 4, 17) },
            new() { PersonalNumber="PN-000006", FirstName="Αναστασία", LastName="Κωνσταντίνου", FatherName="Χρήστος", Birthdate=new DateTime(1974, 9, 25), LastVisitDate=new DateTime(2025, 7, 10), LastEndoscopyDate=new DateTime(2025, 1, 28) },
            new() { PersonalNumber="PN-000007", FirstName="Σπυρίδων",  LastName="Καραγιάννης", FatherName="Νικόλαος", Birthdate=new DateTime(1981, 2, 6),  LastVisitDate=new DateTime(2025, 10, 2), LastEndoscopyDate=new DateTime(2022, 9, 9) },
            new() { PersonalNumber="PN-000008", FirstName="Κατερίνα",  LastName="Δημητρίου",  FatherName="Γεώργιος", Birthdate=new DateTime(1990, 4, 3),  LastVisitDate=new DateTime(2025, 3, 19), LastEndoscopyDate=new DateTime(2025, 3, 19) },
            new() { PersonalNumber="PN-000009", FirstName="Χρήστος",   LastName="Βασιλείου",  FatherName="Θεόδωρος", Birthdate=new DateTime(1963, 12, 1), LastVisitDate=new DateTime(2025, 5, 22), LastEndoscopyDate=new DateTime(2021, 11, 30) },
            new() { PersonalNumber="PN-000010", FirstName="Ιωάννα",    LastName="Λαμπροπούλου", FatherName="Αντώνιος", Birthdate=new DateTime(1988, 8, 14), LastVisitDate=null, LastEndoscopyDate=null },
            new() { PersonalNumber="PN-000011", FirstName="Πέτρος",    LastName="Μιχαηλίδης", FatherName="Ευάγγελος", Birthdate=new DateTime(1971, 6, 27), LastVisitDate=new DateTime(2025, 2, 8),  LastEndoscopyDate=new DateTime(2024, 2, 8) },
            new() { PersonalNumber="PN-000012", FirstName="Δήμητρα",   LastName="Χατζή",      FatherName="Σωτήριος", Birthdate=new DateTime(1995, 10, 5), LastVisitDate=new DateTime(2025, 4, 11), LastEndoscopyDate=new DateTime(2025, 4, 11) },

            new() { PersonalNumber="PN-000013", FirstName="Αλέξανδρος", LastName="Κοσμάς",     FatherName="Μιχαήλ",   Birthdate=new DateTime(1979, 11, 20), LastVisitDate=new DateTime(2025, 11, 30), LastEndoscopyDate=new DateTime(2025, 11, 15) },
            new() { PersonalNumber="PN-000014", FirstName="Σοφία",      LastName="Παπαϊωάννου", FatherName="Γεώργιος", Birthdate=new DateTime(1983, 2, 17),  LastVisitDate=new DateTime(2025, 9, 1),  LastEndoscopyDate=new DateTime(2025, 8, 20) },
            new() { PersonalNumber="PN-000015", FirstName="Αθανάσιος",  LastName="Χριστοδούλου", FatherName="Κωνσταντίνος", Birthdate=new DateTime(1955, 3, 4), LastVisitDate=new DateTime(2025, 1, 12), LastEndoscopyDate=new DateTime(2024, 12, 12) },
            new() { PersonalNumber="PN-000016", FirstName="Φωτεινή",    LastName="Οικονόμου",  FatherName="Σταύρος",  Birthdate=new DateTime(1998, 7, 29),  LastVisitDate=null, LastEndoscopyDate=null },
            new() { PersonalNumber="PN-000017", FirstName="Γιάννης",    LastName="Παπαγεωργίου", FatherName="Βασίλης", Birthdate=new DateTime(1967, 9, 8),  LastVisitDate=new DateTime(2025, 10, 18), LastEndoscopyDate=new DateTime(2020, 10, 18) },
            new() { PersonalNumber="PN-000018", FirstName="Αγγελική",   LastName="Κυριακίδου", FatherName="Νικόλαος", Birthdate=new DateTime(1976, 12, 22), LastVisitDate=new DateTime(2025, 7, 5),  LastEndoscopyDate=new DateTime(2025, 7, 5) },
            new() { PersonalNumber="PN-000019", FirstName="Ευάγγελος",  LastName="Παναγιωτόπουλος", FatherName="Ιωάννης", Birthdate=new DateTime(1989, 5, 16), LastVisitDate=new DateTime(2025, 6, 2), LastEndoscopyDate=new DateTime(2025, 5, 20) },
            new() { PersonalNumber="PN-000020", FirstName="Δέσποινα",   LastName="Καλογεροπούλου", FatherName="Πέτρος", Birthdate=new DateTime(1991, 1, 11), LastVisitDate=new DateTime(2025, 3, 27), LastEndoscopyDate=null },
            new() { PersonalNumber="PN-000021", FirstName="Σταύρος",    LastName="Τριανταφύλλου", FatherName="Χαράλαμπος", Birthdate=new DateTime(1970, 4, 30), LastVisitDate=new DateTime(2025, 2, 14), LastEndoscopyDate=new DateTime(2023, 2, 14) },
            new() { PersonalNumber="PN-000022", FirstName="Άννα",       LastName="Σπυροπούλου", FatherName="Δημήτριος", Birthdate=new DateTime(1984, 10, 9), LastVisitDate=new DateTime(2025, 12, 10), LastEndoscopyDate=new DateTime(2025, 12, 10) },
            new() { PersonalNumber="PN-000023", FirstName="Μιχάλης",    LastName="Ράπτης",     FatherName="Σωκράτης", Birthdate=new DateTime(1959, 6, 21),  LastVisitDate=new DateTime(2025, 8, 29), LastEndoscopyDate=new DateTime(2019, 8, 29) },
            new() { PersonalNumber="PN-000024", FirstName="Παρασκευή",  LastName="Μακρή",      FatherName="Αναστάσιος", Birthdate=new DateTime(1993, 9, 13), LastVisitDate=new DateTime(2025, 5, 6),  LastEndoscopyDate=new DateTime(2025, 5, 6) },

            new() { PersonalNumber="PN-000025", FirstName="Θεοδώρα",    LastName="Κωνσταντάκη", FatherName="Εμμανουήλ", Birthdate=new DateTime(1977, 2, 28), LastVisitDate=new DateTime(2025, 4, 9),  LastEndoscopyDate=new DateTime(2024, 4, 9) },
            new() { PersonalNumber="PN-000026", FirstName="Ιάσων",      LastName="Μάρκου",     FatherName="Αριστείδης", Birthdate=new DateTime(2000, 12, 3), LastVisitDate=new DateTime(2025, 10, 5), LastEndoscopyDate=null },
            new() { PersonalNumber="PN-000027", FirstName="Ραφαήλ",     LastName="Τσάκος",     FatherName="Παναγιώτης", Birthdate=new DateTime(1965, 1, 15), LastVisitDate=new DateTime(2025, 7, 23), LastEndoscopyDate=new DateTime(2025, 7, 23) },
            new() { PersonalNumber="PN-000028", FirstName="Ναταλία",    LastName="Καρρά",      FatherName="Χρήστος", Birthdate=new DateTime(1982, 8, 8),   LastVisitDate=new DateTime(2025, 9, 26), LastEndoscopyDate=new DateTime(2022, 9, 26) },
            new() { PersonalNumber="PN-000029", FirstName="Διονύσης",   LastName="Μανιάτης",   FatherName="Αλέκος",   Birthdate=new DateTime(1973, 3, 19),  LastVisitDate=new DateTime(2025, 6, 17), LastEndoscopyDate=new DateTime(2025, 6, 17) },
            new() { PersonalNumber="PN-000030", FirstName="Λένα",       LastName="Αλεξίου",    FatherName="Σπύρος",   Birthdate=new DateTime(1996, 4, 26),  LastVisitDate=new DateTime(2025, 2, 2),  LastEndoscopyDate=null },
            new() { PersonalNumber="PN-000031", FirstName="Βασιλική",   LastName="Γαλάνη",     FatherName="Θανάσης", Birthdate=new DateTime(1987, 11, 7),  LastVisitDate=new DateTime(2025, 11, 21), LastEndoscopyDate=new DateTime(2025, 11, 21) },
            new() { PersonalNumber="PN-000032", FirstName="Χαρά",       LastName="Πετρίδη",    FatherName="Ανδρέας", Birthdate=new DateTime(1972, 5, 5),   LastVisitDate=new DateTime(2025, 1, 28), LastEndoscopyDate=new DateTime(2021, 1, 28) },
            new() { PersonalNumber="PN-000033", FirstName="Ανδρέας",    LastName="Κεφαλάς",    FatherName="Ευθύμιος", Birthdate=new DateTime(1956, 10, 30), LastVisitDate=new DateTime(2025, 8, 1),  LastEndoscopyDate=new DateTime(2025, 8, 1) },
            new() { PersonalNumber="PN-000034", FirstName="Παναγιώτα",  LastName="Ζαχαρίου",   FatherName="Χρήστος", Birthdate=new DateTime(1999, 9, 9),   LastVisitDate=new DateTime(2025, 3, 3),  LastEndoscopyDate=null },
            new() { PersonalNumber="PN-000035", FirstName="Λουκάς",     LastName="Παπακωνσταντίνου", FatherName="Κυριάκος", Birthdate=new DateTime(1961, 7, 7), LastVisitDate=new DateTime(2025, 12, 2), LastEndoscopyDate=new DateTime(2024, 12, 2) },
            new() { PersonalNumber="PN-000036", FirstName="Ιφιγένεια",  LastName="Μελά",       FatherName="Νικόλαος", Birthdate=new DateTime(1980, 12, 31), LastVisitDate=new DateTime(2025, 9, 12), LastEndoscopyDate=new DateTime(2025, 9, 12) },
        ];

    }
}

