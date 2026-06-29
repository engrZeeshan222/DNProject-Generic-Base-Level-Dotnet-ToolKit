using GenericToolKit.Domain.Entities;
using GenericToolKit.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GenericToolKit.Domain.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplyQueryFilters<T>(
        this IQueryable<T> query,
        BaseFilters filters) where T : BaseEntity
    {
        if (filters is null)
            return query;

        if (filters.AsNoTracking)
            query = query.AsNoTracking();

        if (filters.IgnoreAutoIncludes)
            query = query.IgnoreAutoIncludes();

        if (filters.IncludeDeleted || filters.IgnoreTenantFilter)
            query = query.IgnoreQueryFilters();

        if (!filters.IncludeDeleted)
            query = query.Where(x => x.IsDeleted != true);

        if (!filters.IgnoreTenantFilter && filters.TenantId > 0)
            query = query.Where(x => x.TenantId == filters.TenantId);

        if (filters.Id > 0)
            query = query.Where(x => x.Id == filters.Id);

        if (filters.CreatedBy > 0)
            query = query.Where(x => x.CreatedBy == filters.CreatedBy);

        if (filters.UpdatedBy > 0)
            query = query.Where(x => x.UpdatedBy == filters.UpdatedBy);

        if (filters.DeletedBy > 0)
            query = query.Where(x => x.DeletedBy == filters.DeletedBy);

        if (filters.StartDate.HasValue)
            query = query.Where(x => x.CreatedOn >= filters.StartDate.Value);

        if (filters.EndDate.HasValue)
            query = query.Where(x => x.CreatedOn <= filters.EndDate.Value);

        query = ApplySorting(query, filters);

        if (filters.ApplyPagination)
            query = query.Skip(filters.Skip).Take(filters.Take);

        return query;
    }

    private static IQueryable<T> ApplySorting<T>(
        IQueryable<T> query,
        BaseFilters filters) where T : BaseEntity
    {
        if (filters.OrderExpressions.Count > 0)
            return ApplyExpressionSorting(query, filters.OrderExpressions);

        if (!string.IsNullOrWhiteSpace(filters.SortBy))
            return query.OrderBy(x => EF.Property<object>(x, filters.SortBy));

        return query;
    }

    private static IQueryable<T> ApplyExpressionSorting<T>(
        IQueryable<T> query,
        List<OrderExpression> orderExpressions) where T : BaseEntity
    {
        IOrderedQueryable<T>? orderedQuery = null;

        foreach (var order in orderExpressions)
        {
            if (order.Selector is null)
                continue;

            orderedQuery = order.OrderType switch
            {
                OrderTypeEnum.OrderBy =>
                    Queryable.OrderBy(query, (dynamic)order.Selector),

                OrderTypeEnum.OrderByDescending =>
                    Queryable.OrderByDescending(query, (dynamic)order.Selector),

                OrderTypeEnum.ThenBy when orderedQuery is not null =>
                    Queryable.ThenBy(orderedQuery, (dynamic)order.Selector),

                OrderTypeEnum.ThenByDescending when orderedQuery is not null =>
                    Queryable.ThenByDescending(orderedQuery, (dynamic)order.Selector),

                _ => orderedQuery
            };
        }

        return orderedQuery ?? query;
    }
}