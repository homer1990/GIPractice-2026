namespace GIPractice.Core.Enums;

[Flags]
public enum LabType
{
    None = 0,
    Pathology = 1 << 0,
    Microbiology = 1 << 1,
    Biochemistry = 1 << 2,
    Imaging = 1 << 3,
    BreathTests = 1 << 4,
    Other = 1 << 7
}
