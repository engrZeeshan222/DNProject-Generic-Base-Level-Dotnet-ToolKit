using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Patient.Infra.Data;

public class PatientEntityConfiguration : IEntityTypeConfiguration<Domain.Entities.Patient>
{
    // Configures EF Core entity mappings
    public void Configure(EntityTypeBuilder<Domain.Entities.Patient> builder)
    {

        builder.ToTable("Patients");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.MRN)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.PatientCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.DateOfBirth)
            .IsRequired();

        builder.Property(p => p.Gender)
            .IsRequired();

        builder.Property(p => p.Phone)
            .HasMaxLength(20);

        builder.Property(p => p.Email)
            .HasMaxLength(100);

        builder.Property(p => p.BloodType)
            .HasMaxLength(10);

        builder.Property(p => p.Allergies)
            .HasMaxLength(500);

        builder.Property(p => p.MedicalNotes)
            .HasMaxLength(2000);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.OwnsOne(p => p.Address, address =>
        {
            address.Property(a => a.Street).HasMaxLength(200).HasColumnName("Address_Street");
            address.Property(a => a.City).HasMaxLength(100).HasColumnName("Address_City");
            address.Property(a => a.State).HasMaxLength(100).HasColumnName("Address_State");
            address.Property(a => a.ZipCode).HasMaxLength(20).HasColumnName("Address_ZipCode");
            address.Property(a => a.Country).HasMaxLength(100).HasColumnName("Address_Country");
        });

        builder.OwnsOne(p => p.EmergencyContact, contact =>
        {
            contact.Property(c => c.Name).HasMaxLength(100).HasColumnName("EmergencyContact_Name");
            contact.Property(c => c.Relationship).HasMaxLength(50).HasColumnName("EmergencyContact_Relationship");
            contact.Property(c => c.Phone).HasMaxLength(20).HasColumnName("EmergencyContact_Phone");
            contact.Property(c => c.Email).HasMaxLength(100).HasColumnName("EmergencyContact_Email");
        });

        builder.HasMany(p => p.Appointments)
            .WithOne(a => a.Patient)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.MRN, p.TenantId })
            .IsUnique()
            .HasDatabaseName("IX_Patients_MRN_TenantId");

        builder.HasIndex(p => p.PatientCode)
            .HasDatabaseName("IX_Patients_PatientCode");

        builder.HasIndex(p => p.Email)
            .HasDatabaseName("IX_Patients_Email");

        builder.HasIndex(p => p.TenantId)
            .HasDatabaseName("IX_Patients_TenantId");

        builder.HasIndex(p => p.IsDeleted)
            .HasDatabaseName("IX_Patients_IsDeleted");

        builder.HasIndex(p => new { p.TenantId, p.IsActive, p.IsDeleted })
            .HasDatabaseName("IX_Patients_TenantId_IsActive_IsDeleted");

        builder.Ignore(p => p.FullName);
        builder.Ignore(p => p.Age);
    }
}

