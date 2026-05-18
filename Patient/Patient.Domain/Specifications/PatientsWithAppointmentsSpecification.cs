using System.Linq.Expressions;

namespace Patient.Domain.Specifications;

public class PatientsWithAppointmentsSpecification : BasePatientSpecification
{
    // Builds a specification for patients that have appointments
    public PatientsWithAppointmentsSpecification(DateTime? fromDate = null)
    {
        if (fromDate.HasValue)
        {
            WhereExpression = p => p.Appointments.Any(a => a.AppointmentDateTime >= fromDate.Value);
        }
        else
        {
            WhereExpression = p => p.Appointments.Any();
        }

        AddInclude(p => p.Appointments);
        AddOrderBy(query => query.OrderByDescending(p => p.CreatedOn));
    }
}

