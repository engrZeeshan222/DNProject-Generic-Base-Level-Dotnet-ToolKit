using System.Linq.Expressions;

namespace Patient.Domain.Specifications;

public class ActivePatientsSpecification : BasePatientSpecification
{
    // Builds a specification for active patients
    public ActivePatientsSpecification()
    {
        WhereExpression = p => p.IsActive == true;
        AddOrderBy(query => query.OrderBy(p => p.LastName).ThenBy(p => p.FirstName));
    }
}

