namespace GIPractice.Core.Entities;

public class OrganAreaOrgan
{
    public int OrganId { get; set; }
    public Organ Organ { get; set; } = null!;

    public int OrganAreaId { get; set; }
    public OrganArea OrganArea { get; set; } = null!;
}
