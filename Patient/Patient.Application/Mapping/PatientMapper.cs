using Patient.Application.DTOs;
using Patient.Domain.ValueObjects;

namespace Patient.Application.Mapping;

public static class PatientMapper
{
    // Maps to dto
    public static PatientDto MapToDto(Domain.Entities.Patient entity)
    {
        return new PatientDto
        {
            Id = entity.Id,
            MRN = entity.MRN,
            PatientCode = entity.PatientCode,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            FullName = entity.FullName,
            DateOfBirth = entity.DateOfBirth,
            Age = entity.Age,
            Gender = entity.Gender,
            Phone = entity.Phone,
            Email = entity.Email,
            IsActive = entity.IsActive,
            BloodType = entity.BloodType,
            Allergies = entity.Allergies,
            MedicalNotes = entity.MedicalNotes,
            Address = new AddressDto
            {
                Street = entity.Address.Street,
                City = entity.Address.City,
                State = entity.Address.State,
                ZipCode = entity.Address.ZipCode,
                Country = entity.Address.Country,
                FullAddress = entity.Address.FullAddress
            },
            EmergencyContact = new EmergencyContactDto
            {
                Name = entity.EmergencyContact.Name,
                Relationship = entity.EmergencyContact.Relationship,
                Phone = entity.EmergencyContact.Phone,
                Email = entity.EmergencyContact.Email
            },

            TenantId = entity.TenantId,
            CreatedBy = entity.CreatedBy,
            CreatedOn = entity.CreatedOn,
            UpdatedBy = entity.UpdatedBy,
            UpdatedOn = entity.UpdatedOn,
            IsDeleted = entity.IsDeleted,
            DeletedBy = entity.DeletedBy,
            DeletedOn = entity.DeletedOn
        };
    }

    // Maps to entity
    public static Domain.Entities.Patient MapToEntity(CreatePatientRequest request)
    {
        return new Domain.Entities.Patient
        {
            MRN = request.MRN,
            PatientCode = request.PatientCode,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Phone = request.Phone,
            Email = request.Email,
            BloodType = request.BloodType,
            Allergies = request.Allergies,
            MedicalNotes = request.MedicalNotes,
            IsActive = true,
            Address = new Address
            {
                Street = request.Address.Street,
                City = request.Address.City,
                State = request.Address.State,
                ZipCode = request.Address.ZipCode,
                Country = request.Address.Country
            },
            EmergencyContact = new EmergencyContact
            {
                Name = request.EmergencyContact.Name,
                Relationship = request.EmergencyContact.Relationship,
                Phone = request.EmergencyContact.Phone,
                Email = request.EmergencyContact.Email
            }
        };
    }
}

