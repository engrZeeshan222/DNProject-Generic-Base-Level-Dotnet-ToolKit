using System.Linq.Expressions;

namespace Patient.Domain.Specifications;

public class PatientByMRNSpecification : BasePatientSpecification
{
    // Builds a specification to find a patient by MRN
    public PatientByMRNSpecification(string mrn, bool includeAppointments = false)
    {
        WhereExpression = p => p.MRN == mrn;

        if (includeAppointments)
        {
            AddInclude(p => p.Appointments);
        }
    }
}

