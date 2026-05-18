using System.Linq.Expressions;

namespace GenericToolKit.Domain.Models
{

    public class BaseFilters
    {

        public int Id { get; set; } = 0;

        public int TenantId { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public int DeleteBy { get; set; }

        public bool IsAsNoTracking { get; set; } = true;

        public bool IgnoreActiveCheck { get; set; } = false;

        public bool IgnoreTenantCheck { get; set; } = false;

        public bool ApplyPagination { get; set; } = false;

        private int skip;
        private int take;

        public int? Take
        {
            get
            {
                return take == 0 ? 20 : take;
            }
            set
            {
                take = value ?? 0;
            }
        }

        public int? Skip
        {
            get
            {
                return skip == 0 ? 0 : skip;
            }
            set
            {
                skip = value ?? 0;
            }
        }

        public string ApplySorting { get; set; }

        public List<OrderExpression> OrderExpressions { get; set; } = new List<OrderExpression>();

        public bool IsIgnoreAutoIncludes { get; set; } = false;

        public bool IncludeSoftDeletedEntitiesAlso { get; set; } = false;

        public DateTime? StartDate { get; set; } = null;

        public DateTime? EndDate { get; set; } = null;
    }

    public class OrderExpression
    {

        public OrderTypeEnum OrderType { get; set; }

        public Expression<Func<IQueryable, object>> Selector { get; set; }
    }

    public enum OrderTypeEnum
    {
        OrderBy = 1,
        OrderByDescending = 2,
        ThenBy = 3,
        ThenByDescending = 4
    }
}

