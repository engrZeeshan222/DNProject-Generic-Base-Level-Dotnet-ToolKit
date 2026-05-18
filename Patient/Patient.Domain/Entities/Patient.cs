using GenericToolKit.Domain.Entities;
using Patient.Domain.Enums;
using Patient.Domain.ValueObjects;

namespace Patient.Domain.Entities;

public class Patient : BaseEntity
{

    public string MRN { get; set; } = string.Empty;

    public string PatientCode { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public Address Address { get; set; } = new Address();

    public EmergencyContact EmergencyContact { get; set; } = new EmergencyContact();

    public bool IsActive { get; set; } = true;

    public string? BloodType { get; set; }

    public string? Allergies { get; set; }

    public string? MedicalNotes { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public string FullName => $"{FirstName} {LastName}";

    public int Age
    {
        get
        {
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

    // Activates the patient record
    public void Activate()
    {
        if (IsDeleted == true)
        {
            throw new InvalidOperationException("Cannot activate a deleted patient. Please restore first.");
        }
        IsActive = true;
    }

    // Deactivates the patient record
    public void Deactivate()
    {
        IsActive = false;
    }

    // Validates MRN format
    public bool IsValidMRN()
    {
        return !string.IsNullOrWhiteSpace(MRN) && MRN.Length >= 5;
    }

    // Validates patient age
    public bool IsValidAge()
    {
        return DateOfBirth < DateTime.Today && Age >= 0 && Age < 150;
    }

    // Validates required patient fields
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(FirstName) &&
               !string.IsNullOrWhiteSpace(LastName) &&
               IsValidMRN() &&
               IsValidAge();
    }
}

