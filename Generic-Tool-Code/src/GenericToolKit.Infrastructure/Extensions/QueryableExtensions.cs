using GenericToolKit.Domain.Entities;
using GenericToolKit.Domain.Extensions;
using GenericToolKit.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GenericToolKit.Domain.Extensions
{

    public static class QueryableExtensions
    {

        public static IQueryable<T> ApplyQueryFilters<T>(this IQueryable<T> sourceQuery, BaseFilters filters) where T : BaseEntity
        {
            try
            {
                if (!filters.IsValidObject())
                    return sourceQuery;

                sourceQuery = ApplyDefaultFilters(sourceQuery);

                if (!filters.IgnoreTenantCheck && filters.TenantId > 0)
                {
                    sourceQuery = sourceQuery.Where(x => x.TenantId == filters.TenantId);
                }

                if (filters.CreatedBy > 0)
                {
                    sourceQuery = sourceQuery.Where(x => x.CreatedBy == filters.CreatedBy);
                }

                if (filters.UpdatedBy > 0)
                {
                    sourceQuery = sourceQuery.Where(x => x.UpdatedBy == filters.UpdatedBy);
                }

                if (filters.DeleteBy > 0)
                {
                    sourceQuery = sourceQuery.Where(x => x.DeletedBy == filters.DeleteBy);
                }

                if (filters.IsAsNoTracking)
                {
                    sourceQuery = sourceQuery.AsNoTracking();
                }

                if (filters.IgnoreActiveCheck)
                {
                    sourceQuery = sourceQuery.Where(x => (x.IsDeleted == true));
                }

                if(filters.ApplyPagination)
                    sourceQuery = sourceQuery.Skip(filters.Skip.GetValueOrDefault()).Take(filters.Take.GetValueOrDefault());

                if (!string.IsNullOrWhiteSpace(filters.ApplySorting) || filters.OrderExpressions.Count != 0)
                {

                    if (filters.OrderExpressions.Count != 0)
                    {
                        IOrderedQueryable<T> orderedQuery = null;
                        foreach (var expression in filters.OrderExpressions)
                        {
                            if(expression.OrderType == OrderTypeEnum.OrderBy)
                            {
                                orderedQuery = Queryable.OrderBy((IQueryable<T>)sourceQuery, (dynamic)expression.Selector);
                            }else if(expression.OrderType == OrderTypeEnum.OrderByDescending)
                            {
                                orderedQuery = Queryable.OrderByDescending((IQueryable<T>)sourceQuery, (dynamic)expression.Selector);
                            }else if(expression.OrderType == OrderTypeEnum.ThenBy)
                            {
                                orderedQuery = Queryable.ThenBy((IOrderedQueryable<T>)sourceQuery, (dynamic)expression.Selector);
                            }else  if(expression.OrderType == OrderTypeEnum.ThenByDescending)
                            {
                                orderedQuery = Queryable.ThenByDescending((IOrderedQueryable<T>)sourceQuery, (dynamic)expression.Selector);
                            }
                        }
                        if (orderedQuery != null)
                            sourceQuery = orderedQuery;
                    }

                    else if (!string.IsNullOrWhiteSpace(filters.ApplySorting))
                    {
                        sourceQuery = sourceQuery.OrderBy(x => EF.Property<object>(x, filters.ApplySorting));
                    }
                }

                if(filters.IncludeSoftDeletedEntitiesAlso)
                {
                    sourceQuery = sourceQuery.Where(x => x.IsDeleted == true);
                }

                if (filters.StartDate.HasValue)
                {
                    sourceQuery = sourceQuery.Where(x => x.CreatedOn >= filters.StartDate.Value);
                }
                if(filters.EndDate.HasValue)
                {
                    sourceQuery = sourceQuery.Where(x => x.CreatedOn <= filters.EndDate.Value);
                }
                return sourceQuery;
            }
            catch (Exception)
            {

                return sourceQuery;
            }
        }

        private static IQueryable<T> ApplyDefaultFilters<T>(IQueryable<T> sourceQuery) where T : BaseEntity
        {
            sourceQuery = sourceQuery.Where(a => a.IsDeleted != true);
            return sourceQuery;
        }
    }
}

