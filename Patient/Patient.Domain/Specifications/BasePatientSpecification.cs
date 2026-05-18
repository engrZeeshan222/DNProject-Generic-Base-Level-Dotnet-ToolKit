using System.Linq.Expressions;
using GenericToolKit.Domain.Interfaces;

namespace Patient.Domain.Specifications;

public abstract class BasePatientSpecification : IBaseSpecification<Entities.Patient>
{
    protected BasePatientSpecification()
    {
        Includes = new List<Expression<Func<Entities.Patient, object>>>();
        IncludeStrings = new List<string>();
    }

    public Expression<Func<Entities.Patient, bool>> WhereExpression { get; protected set; } = null!;

    public List<Expression<Func<Entities.Patient, object>>> Includes { get; protected set; }

    public List<string> IncludeStrings { get; protected set; }

    public Func<IQueryable<Entities.Patient>, IOrderedQueryable<Entities.Patient>> OrderByDelegate { get; protected set; } = null!;

    public bool IsAsNoTracking { get; protected set; } = true;

    // Adds include
    protected void AddInclude(Expression<Func<Entities.Patient, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    // Adds include
    protected void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    // Adds order by
    protected void AddOrderBy(Func<IQueryable<Entities.Patient>, IOrderedQueryable<Entities.Patient>> orderByDelegate)
    {
        OrderByDelegate = orderByDelegate;
    }

    // Sets tracking
    protected void SetTracking(bool isTracking)
    {
        IsAsNoTracking = !isTracking;
    }
}

