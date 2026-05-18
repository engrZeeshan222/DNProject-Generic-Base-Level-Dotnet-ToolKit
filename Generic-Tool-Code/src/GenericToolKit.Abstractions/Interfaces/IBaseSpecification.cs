using GenericToolKit.Domain.Entities;
using System.Linq.Expressions;

namespace GenericToolKit.Domain.Interfaces
{

    public interface IBaseSpecification<T> where T : BaseEntity
    {

        Expression<Func<T, bool>> WhereExpression { get; }

        List<Expression<Func<T, object>>> Includes { get; }

        List<string> IncludeStrings { get; }

        Func<IQueryable<T>, IOrderedQueryable<T>> OrderByDelegate { get; }

        bool IsAsNoTracking { get; }
    }
}

