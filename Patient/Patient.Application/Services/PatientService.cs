using GenericToolKit.Application.Services;
using GenericToolKit.Domain.Interfaces;
using GenericToolKit.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Patient.Application.DTOs;
using Patient.Application.Mapping;

namespace Patient.Application.Services;

public class PatientService : GenericService<Domain.Entities.Patient>, IPatientService
{
    private readonly IGenericRepository<Domain.Entities.Patient> _repository;
    private readonly ILoggedInUser _loggedInUser;

    // Initializes the patient application service
    public PatientService(
        IGenericRepository<Domain.Entities.Patient> repository,
        ILoggedInUser loggedInUser)
        : base(repository, loggedInUser)
    {
        _repository = repository;
        _loggedInUser = loggedInUser;
    }

    // Creates patient
    public async Task<PatientDto> CreatePatientAsync(CreatePatientRequest request, CancellationToken cancellationToken = default)
    {

        var mrnExists = await Any(p => p.MRN == request.MRN && p.TenantId == _loggedInUser.TenantId, cancellationToken);

        if (mrnExists)
        {
            throw new InvalidOperationException($"MRN '{request.MRN}' already exists in this facility.");
        }

        var patient = PatientMapper.MapToEntity(request);

        if (!patient.IsValid())
        {
            throw new InvalidOperationException("Patient data is invalid.");
        }

        var createdPatient = await Add(patient);

        return PatientMapper.MapToDto(createdPatient);
    }

    // Updates patient
    public async Task<PatientDto> UpdatePatientAsync(UpdatePatientRequest request, CancellationToken cancellationToken = default)
    {

        var existingPatient = await GetByIdQuery(request.Id, detached: false).SingleOrDefaultAsync(cancellationToken);

        if (existingPatient == null)
        {
            throw new InvalidOperationException($"Patient with ID {request.Id} not found.");
        }

        existingPatient.FirstName = request.FirstName;
        existingPatient.LastName = request.LastName;
        existingPatient.DateOfBirth = request.DateOfBirth;
        existingPatient.Gender = request.Gender;
        existingPatient.Phone = request.Phone;
        existingPatient.Email = request.Email;
        existingPatient.BloodType = request.BloodType;
        existingPatient.Allergies = request.Allergies;
        existingPatient.MedicalNotes = request.MedicalNotes;

        existingPatient.Address = new Domain.ValueObjects.Address
        {
            Street = request.Address.Street,
            City = request.Address.City,
            State = request.Address.State,
            ZipCode = request.Address.ZipCode,
            Country = request.Address.Country
        };

        existingPatient.EmergencyContact = new Domain.ValueObjects.EmergencyContact
        {
            Name = request.EmergencyContact.Name,
            Relationship = request.EmergencyContact.Relationship,
            Phone = request.EmergencyContact.Phone,
            Email = request.EmergencyContact.Email
        };

        if (!existingPatient.IsValid())
        {
            throw new InvalidOperationException("Updated patient data is invalid.");
        }

        await UpdateOne(existingPatient, cancellationToken);

        return PatientMapper.MapToDto(existingPatient);
    }

    // Gets patient by id
    public async Task<PatientDto?> GetPatientByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var patient = await GetByIdQuery(id, detached: true).SingleOrDefaultAsync(cancellationToken);

        return patient == null ? null : PatientMapper.MapToDto(patient);
    }

    // Gets active patients
    public async Task<List<PatientDto>> GetActivePatientsAsync(CancellationToken cancellationToken = default)
    {

        var filters = new BaseFilters
        {
            IsAsNoTracking = true,
            TenantId = _loggedInUser.TenantId
        };

        var patients = await GetAll(filters);

        return patients
            .Where(p => p.IsActive)
            .Select(PatientMapper.MapToDto)
            .ToList();
    }

    // Searches patients
    public async Task<List<PatientDto>> SearchPatientsAsync(string searchTerm, CancellationToken cancellationToken = default)
    {

        var lowerSearchTerm = searchTerm.ToLower();
        var query = Find(p =>
            p.FirstName.ToLower().Contains(lowerSearchTerm) ||
            p.LastName.ToLower().Contains(lowerSearchTerm),
            findOptions: null);

        var patients = await query.ToListAsync(cancellationToken);

        return patients.Select(PatientMapper.MapToDto).ToList();
    }

    // Activates a patient by id
    public async Task<bool> ActivatePatientAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var patient = await GetByIdQuery(patientId, detached: false).SingleOrDefaultAsync(cancellationToken);

        if (patient == null)
        {
            throw new InvalidOperationException($"Patient with ID {patientId} not found.");
        }

        patient.Activate();

        await UpdateOne(patient, cancellationToken);

        return true;
    }

    // Deactivates a patient by id
    public async Task<bool> DeactivatePatientAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var patient = await GetByIdQuery(patientId, detached: false).SingleOrDefaultAsync(cancellationToken);

        if (patient == null)
        {
            throw new InvalidOperationException($"Patient with ID {patientId} not found.");
        }

        patient.Deactivate();

        await UpdateOne(patient, cancellationToken);

        return true;
    }

    // Gets patient change history
    public async Task<string> GetPatientChangeHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var patient = await GetByIdQuery(patientId, detached: false).SingleOrDefaultAsync(cancellationToken);

        if (patient == null)
        {
            throw new InvalidOperationException($"Patient with ID {patientId} not found.");
        }

        patient.Phone = "000-000-0000";

        var changeJson = await DetectChange(patient);

        await RestoreOriginalValuesAsync(patient, new List<string> { "Phone" });

        return changeJson;
    }

    // Creates patients bulk
    public async Task<List<PatientDto>> CreatePatientsBulkAsync(List<CreatePatientRequest> requests, CancellationToken cancellationToken = default)
    {

        var patients = requests.Select(PatientMapper.MapToEntity).ToList();

        foreach (var patient in patients)
        {
            if (!patient.IsValid())
            {
                throw new InvalidOperationException($"Invalid patient data: {patient.MRN}");
            }
        }

        var success = await AddMany(patients);

        if (!success)
        {
            throw new InvalidOperationException("Failed to create patients in bulk");
        }

        return patients.Select(PatientMapper.MapToDto).ToList();
    }

    // Creates or updates a patient (upsert)
    public async Task<PatientDto> SaveOrUpdatePatientAsync(CreatePatientRequest request, CancellationToken cancellationToken = default)
    {

        var patient = PatientMapper.MapToEntity(request);

        var existingPatient = await FindOne(p => p.MRN == request.MRN && p.TenantId == _loggedInUser.TenantId, findOptions: null);

        if (existingPatient != null)
        {

            patient.Id = existingPatient.Id;
            patient.FirstName = request.FirstName;
            patient.LastName = request.LastName;
            patient.DateOfBirth = request.DateOfBirth;
            patient.Gender = request.Gender;
            patient.Phone = request.Phone;
            patient.Email = request.Email;
            patient.BloodType = request.BloodType;
            patient.Allergies = request.Allergies;
            patient.MedicalNotes = request.MedicalNotes;
            patient.Address = new Domain.ValueObjects.Address
            {
                Street = request.Address.Street,
                City = request.Address.City,
                State = request.Address.State,
                ZipCode = request.Address.ZipCode,
                Country = request.Address.Country
            };
            patient.EmergencyContact = new Domain.ValueObjects.EmergencyContact
            {
                Name = request.EmergencyContact.Name,
                Relationship = request.EmergencyContact.Relationship,
                Phone = request.EmergencyContact.Phone,
                Email = request.EmergencyContact.Email
            };
        }

        var result = await SaveOrUpdate(patient, setAuditProperties: true, shouldSave: true);

        return PatientMapper.MapToDto(result);
    }

    // Gets patients by ids
    public async Task<List<PatientDto>> GetPatientsByIdsAsync(List<int> ids, CancellationToken cancellationToken = default)
    {

        var patients = await ListAsync(ids, cancellationToken);
        return patients.Select(PatientMapper.MapToDto).ToList();
    }

    // Counts patients
    public async Task<int> CountPatientsAsync(System.Linq.Expressions.Expression<Func<Domain.Entities.Patient, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {

        if (predicate == null)
        {

            predicate = p => true;
        }

        return await Count(predicate, cancellationToken);
    }

    // Soft-deletes multiple patients by id
    public async Task<bool> SoftDeletePatientsAsync(List<int> patientIds, CancellationToken cancellationToken = default)
    {

        var patients = await ListAsync(patientIds, cancellationToken);

        if (patients.Count == 0)
        {
            return false;
        }

        return await SoftDeleteMany(patients, cancellationToken);
    }

    // Permanently deletes patients matching a condition
    public async Task<int> HardDeletePatientsByConditionAsync(System.Linq.Expressions.Expression<Func<Domain.Entities.Patient, bool>> predicate, CancellationToken cancellationToken = default)
    {

        return await HardDeleteMany(predicate);
    }

    // Hard-deletes delete patient entity
    public async Task<int> HardDeletePatientEntityAsync(int patientId, CancellationToken cancellationToken = default)
    {

        var patient = await GetByIdQuery(patientId, detached: false).SingleOrDefaultAsync(cancellationToken);

        if (patient == null)
        {
            return 0;
        }

        return await HardDeleteOne(patient);
    }

    // Removes patients list
    public async Task<bool> RemovePatientsListAsync(List<int> patientIds, CancellationToken cancellationToken = default)
    {

        var patients = await ListAsync(patientIds, cancellationToken);

        if (patients.Count == 0)
        {
            return false;
        }

        return await RemoveListOfEntities(patients);
    }

    // Gets patient full json comparison
    public async Task<string> GetPatientFullJsonComparisonAsync(int patientId, CancellationToken cancellationToken = default)
    {

        var patient = await GetByIdQuery(patientId, detached: false).SingleOrDefaultAsync(cancellationToken);

        if (patient == null)
        {
            throw new InvalidOperationException($"Patient with ID {patientId} not found.");
        }

        var originalPhone = patient.Phone;
        var originalEmail = patient.Email;
        patient.Phone = "999-999-9999";
        patient.Email = "changed@example.com";

        var comparisonJson = await LogFullJsonComparison(patient);

        patient.Phone = originalPhone;
        patient.Email = originalEmail;
        await RestoreOriginalValuesAsync(patient, new List<string> { "Phone", "Email" });

        return comparisonJson;
    }

    // Sets patient audit properties
    public async Task<PatientDto> SetPatientAuditPropertiesAsync(int patientId, CancellationToken cancellationToken = default)
    {

        var patient = await GetByIdQuery(patientId, detached: false).SingleOrDefaultAsync(cancellationToken);

        if (patient == null)
        {
            throw new InvalidOperationException($"Patient with ID {patientId} not found.");
        }

        var updatedPatient = await SetAuditPropertiesAsync(patient);

        return PatientMapper.MapToDto(updatedPatient);
    }

    // Gets patients with advanced filters
    public async Task<List<PatientDto>> GetPatientsWithAdvancedFiltersAsync(
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
        CancellationToken cancellationToken = default)
    {

        var filters = new BaseFilters
        {

            IsAsNoTracking = true,
            IncludeSoftDeletedEntitiesAlso = includeSoftDeleted,
            IgnoreTenantCheck = ignoreTenantCheck,

            CreatedBy = createdBy ?? 0,
            UpdatedBy = updatedBy ?? 0,
            DeleteBy = deleteBy ?? 0,

            StartDate = startDate,
            EndDate = endDate,

            ApplyPagination = skip.HasValue || take.HasValue,
            Skip = skip ?? 0,
            Take = take ?? 20,

            ApplySorting = sortBy
        };

        if (!ignoreTenantCheck)
        {
            filters.TenantId = _loggedInUser.TenantId;
        }

        var patients = await GetAll(filters);

        return patients.Select(PatientMapper.MapToDto).ToList();
    }
}

