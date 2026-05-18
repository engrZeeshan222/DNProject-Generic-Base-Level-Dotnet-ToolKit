using GenericToolKit.Domain.Interfaces;
using GenericToolKit.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Patient.Infra.Data;

namespace Patient.Infra.Repositories;

public class PatientRepository : GenericRepository<Domain.Entities.Patient>, IPatientRepository
{
    private readonly PatientDbContext _context;

    // Initializes the patient data repository
    public PatientRepository(PatientDbContext context, ILoggedInUser loggedInUser)
        : base(context, loggedInUser)
    {
        _context = context;
    }

    // Finds by mrn
    public async Task<Domain.Entities.Patient?> FindByMRNAsync(string mrn, CancellationToken cancellationToken = default)
    {

        return await FindOne(p => p.MRN == mrn, null);
    }

    // Finds by email
    public async Task<Domain.Entities.Patient?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await FindOne(p => p.Email == email, null);
    }

    // Finds by patient code
    public async Task<Domain.Entities.Patient?> FindByPatientCodeAsync(string patientCode, CancellationToken cancellationToken = default)
    {
        return await FindOne(p => p.PatientCode == patientCode, null);
    }

    // Gets patients with upcoming appointments
    public async Task<List<Domain.Entities.Patient>> GetPatientsWithUpcomingAppointmentsAsync(
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {

        return await _context.Patients
            .Include(p => p.Appointments.Where(a => a.AppointmentDateTime >= fromDate && a.AppointmentDateTime <= toDate))
            .Where(p => p.Appointments.Any(a => a.AppointmentDateTime >= fromDate && a.AppointmentDateTime <= toDate))
            .ToListAsync(cancellationToken);
    }

    // Checks if mrnunique in tenant
    public async Task<bool> IsMRNUniqueInTenantAsync(string mrn, int tenantId, int? excludePatientId = null, CancellationToken cancellationToken = default)
    {

        if (excludePatientId.HasValue)
        {
            return !await Any(p => p.MRN == mrn && p.TenantId == tenantId && p.Id != excludePatientId.Value, cancellationToken);
        }

        return !await Any(p => p.MRN == mrn && p.TenantId == tenantId, cancellationToken);
    }

    // Gets patients by age range
    public async Task<List<Domain.Entities.Patient>> GetPatientsByAgeRangeAsync(int minAge, int maxAge, CancellationToken cancellationToken = default)
    {
        var maxBirthDate = DateTime.Today.AddYears(-minAge);
        var minBirthDate = DateTime.Today.AddYears(-maxAge - 1);

        var query = Find(p => p.DateOfBirth >= minBirthDate && p.DateOfBirth <= maxBirthDate);

        return await query.ToListAsync(cancellationToken);
    }

    // Searches by name
    public async Task<List<Domain.Entities.Patient>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {

        var lowerSearchTerm = searchTerm.ToLower();

        var query = Find(p =>
            p.FirstName.ToLower().Contains(lowerSearchTerm) ||
            p.LastName.ToLower().Contains(lowerSearchTerm));

        return await query
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync(cancellationToken);
    }
}

