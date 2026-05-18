using GenericToolKit.Application.Services;
using Patient.Application.DTOs;
using System.Linq.Expressions;

namespace Patient.Application.Services;

public interface IPatientService : IGenericService<Domain.Entities.Patient>
{

    // Creates patient
    Task<PatientDto> CreatePatientAsync(CreatePatientRequest request, CancellationToken cancellationToken = default);

    // Updates patient
    Task<PatientDto> UpdatePatientAsync(UpdatePatientRequest request, CancellationToken cancellationToken = default);

    // Gets patient by id
    Task<PatientDto?> GetPatientByIdAsync(int id, CancellationToken cancellationToken = default);

    // Gets active patients
    Task<List<PatientDto>> GetActivePatientsAsync(CancellationToken cancellationToken = default);

    // Searches patients
    Task<List<PatientDto>> SearchPatientsAsync(string searchTerm, CancellationToken cancellationToken = default);

    // Activate patient
    Task<bool> ActivatePatientAsync(int patientId, CancellationToken cancellationToken = default);

    // Deactivate patient
    Task<bool> DeactivatePatientAsync(int patientId, CancellationToken cancellationToken = default);

    // Gets patient change history
    Task<string> GetPatientChangeHistoryAsync(int patientId, CancellationToken cancellationToken = default);

    // Creates patients bulk
    Task<List<PatientDto>> CreatePatientsBulkAsync(List<CreatePatientRequest> requests, CancellationToken cancellationToken = default);

    // Saves or updates or update patient
    Task<PatientDto> SaveOrUpdatePatientAsync(CreatePatientRequest request, CancellationToken cancellationToken = default);

    // Gets patients by ids
    Task<List<PatientDto>> GetPatientsByIdsAsync(List<int> ids, CancellationToken cancellationToken = default);

    // Counts patients
    Task<int> CountPatientsAsync(Expression<Func<Domain.Entities.Patient, bool>>? predicate = null, CancellationToken cancellationToken = default);

    // Soft-deletes delete patients
    Task<bool> SoftDeletePatientsAsync(List<int> patientIds, CancellationToken cancellationToken = default);

    // Hard-deletes delete patients by condition
    Task<int> HardDeletePatientsByConditionAsync(Expression<Func<Domain.Entities.Patient, bool>> predicate, CancellationToken cancellationToken = default);

    // Hard-deletes delete patient entity
    Task<int> HardDeletePatientEntityAsync(int patientId, CancellationToken cancellationToken = default);

    // Removes patients list
    Task<bool> RemovePatientsListAsync(List<int> patientIds, CancellationToken cancellationToken = default);

    // Gets patient full json comparison
    Task<string> GetPatientFullJsonComparisonAsync(int patientId, CancellationToken cancellationToken = default);

    // Sets patient audit properties
    Task<PatientDto> SetPatientAuditPropertiesAsync(int patientId, CancellationToken cancellationToken = default);

    Task<List<PatientDto>> GetPatientsWithAdvancedFiltersAsync(
        int? createdBy = null,
        int? updatedBy = null,
        int? deleteBy = null,
        bool ignoreTenantCheck = false,
        string? sortBy = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        bool includeSoftDeleted = false,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default);
}

