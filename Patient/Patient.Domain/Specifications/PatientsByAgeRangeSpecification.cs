using System.Linq.Expressions;

namespace Patient.Domain.Specifications;

public class PatientsByAgeRangeSpecification : BasePatientSpecification
{
    // Builds a specification for patients within an age range
    public PatientsByAgeRangeSpecification(int minAge, int maxAge)
    {
        var maxBirthDate = DateTime.Today.AddYears(-minAge);
        var minBirthDate = DateTime.Today.AddYears(-maxAge - 1);

        WhereExpression = p => p.DateOfBirth >= minBirthDate && p.DateOfBirth <= maxBirthDate;
        AddOrderBy(query => query.OrderBy(p => p.DateOfBirth));
    }
}

