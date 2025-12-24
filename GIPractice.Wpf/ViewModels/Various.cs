using System;
using System.Windows.Media;

namespace GIPractice.Wpf.ViewModels;

public sealed class AppointmentVm
{
    public DateTime Start { get; set; }
    public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(30);

    public string PatientFullName { get; set; } = "";
    public string PatientPhoneNo { get; set; } = "";
    public string AppointmentTypeName { get; set; } = "";

    public Brush TypeAccentBrush { get; set; } = Brushes.Gray;
}

public sealed class AppointmentTypeVm
{
    public string Name { get; set; } = "";
    public int MeanDurationMinutes { get; set; } = 30;
    public Brush AccentBrush { get; set; } = Brushes.DodgerBlue;
}

public sealed class PatientVm : ViewModelBase
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FathersName { get; set; }
    public string? PersonalNumber { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Gender { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }

    private ImageSource? _photoImageSource;
    public ImageSource? PhotoImageSource
    {
        get => _photoImageSource;
        set { _photoImageSource = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsPhotoDirty)); }
    }

    // Baseline: last saved / loaded photo
    private ImageSource? _baselinePhoto;
    public bool IsPhotoDirty => !ReferenceEquals(PhotoImageSource, _baselinePhoto);

    /// Call this right after you load a patient from API/DB (or after successful Save).
    public void AcceptPhotoAsBaseline()
    {
        _baselinePhoto = PhotoImageSource;
        OnPropertyChanged(nameof(IsPhotoDirty));
    }

    /// Called by "ΕΠΑΝΑΦΟΡΑ"
    public void ResetPhotoToBaseline()
    {
        PhotoImageSource = _baselinePhoto; // null for new patients => clears
        OnPropertyChanged(nameof(IsPhotoDirty));
    }

    /// Use this for new patient creation
    public static PatientVm CreateNew()
    {
        var p = new PatientVm();
        p.PhotoImageSource = null;
        p.AcceptPhotoAsBaseline(); // baseline is empty
        return p;
    }
}
