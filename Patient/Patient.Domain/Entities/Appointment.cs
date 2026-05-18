using GenericToolKit.Domain.Entities;

namespace Patient.Domain.Entities;

public class Appointment : BaseEntity
{

    public int PatientId { get; set; }

    public virtual Patient Patient { get; set; } = null!;

    public DateTime AppointmentDateTime { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string DoctorName { get; set; } = string.Empty;

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    public string? Notes { get; set; }

    // Cancel
    public void Cancel()
    {
        if (Status == AppointmentStatus.Completed)
        {
            throw new InvalidOperationException("Cannot cancel a completed appointment.");
        }
        Status = AppointmentStatus.Cancelled;
    }

    // Complete
    public void Complete()
    {
        if (Status == AppointmentStatus.Cancelled)
        {
            throw new InvalidOperationException("Cannot complete a cancelled appointment.");
        }
        Status = AppointmentStatus.Completed;
    }

    // Validates required patient fields
    public bool IsValid()
    {
        return PatientId > 0 &&
               AppointmentDateTime > DateTime.Now &&
               !string.IsNullOrWhiteSpace(Reason) &&
               !string.IsNullOrWhiteSpace(DoctorName);
    }
}

public enum AppointmentStatus
{
    Scheduled = 1,
    Completed = 2,
    Cancelled = 3,
    NoShow = 4
}

