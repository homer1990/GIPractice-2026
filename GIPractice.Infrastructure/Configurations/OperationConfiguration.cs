using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GIPractice.Infrastructure.Configurations;

public class OperationConfiguration : IEntityTypeConfiguration<Operation>
{
    public void Configure(EntityTypeBuilder<Operation> b)
    {
        b.ToTable("Operations");

        b.HasKey(o => o.Id);

        b.Property(o => o.DateAndTimeUtc)
            .IsRequired();

        b.HasOne(o => o.Patient)
            .WithMany(p => p.Operations)
            .HasForeignKey(o => o.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(o => o.Doctor)
            .WithMany()
            .HasForeignKey(o => o.DoctorId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(o => o.File)
            .WithMany()
            .HasForeignKey(o => o.FileId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
