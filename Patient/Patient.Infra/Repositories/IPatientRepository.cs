using GenericToolKit.Domain.Interfaces;

namespace Patient.Infra.Repositories;

public interface IPatientRepository : IGenericRepository<Domain.Entities.Patient>
{

    // Finds by mrn
    Task<Domain.Entities.Patient?> FindByMRNAsync(string mrn, CancellationToken cancellationToken = default);

    // Finds by email
    Task<Domain.Entities.Patient?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

    // Finds by patient code
    Task<Domain.Entities.Patient?> FindByPatientCodeAsync(string patientCode, CancellationToken cancellationToken = default);

    Task<List<Domain.Entities.Patient>> GetPatientsWithUpcomingAppointmentsAsync(
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);

    // Checks if mrnunique in tenant
    Task<bool> IsMRNUniqueInTenantAsync(string mrn, int tenantId, int? excludePatientId = null, CancellationToken cancellationToken = default);

    // Gets patients by age range
    Task<List<Domain.Entities.Patient>> GetPatientsByAgeRangeAsync(int minAge, int maxAge, CancellationToken cancellationToken = default);

    // Searches by name
    Task<List<Domain.Entities.Patient>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
}

