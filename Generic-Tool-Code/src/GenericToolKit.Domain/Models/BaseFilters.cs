using System.Linq.Expressions;

namespace GenericToolKit.Domain.Models;

public class BaseFilters
{
    private int _skip;
    private int _take = 20;

    private string? _sortBy;

    public int Id { get; set; }

    public int TenantId { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public int DeletedBy { get; set; }

    public bool AsNoTracking { get; set; } = true;

    public bool IgnoreTenantFilter { get; set; } = false;

    public bool IncludeInactive { get; set; } = false;

    public bool IncludeDeleted { get; set; } = false;

    public bool ApplyPagination { get; set; }

    // Encapsulated Skip property
    public int Skip
    {
        get => _skip;
        set => _skip = value < 0 ? 0 : value;
    }

    // Encapsulated Take property
    public int Take
    {
        get => _take;
        set => _take = value <= 0 ? 20 : value;
    }

    // Encapsulated SortBy property
    public string? SortBy
    {
        get => _sortBy;
        set => _sortBy = string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    public List<OrderExpression> OrderExpressions { get; set; } = new List<OrderExpression>();

    public bool IgnoreAutoIncludes { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
    public void ApplyDefaultFilters(int tenantId)
    {
        TenantId = tenantId;

        IgnoreTenantFilter = false;

        IncludeDeleted = false;

        IncludeInactive = false;
    }
}

public class OrderExpression
{
    private LambdaExpression? _selector;

    public OrderTypeEnum OrderType { get; set; }

    // Encapsulated Selector property
    public LambdaExpression? Selector
    {
        get => _selector;
        set => _selector = value ?? throw new ArgumentNullException(nameof(Selector));
    }
}

public enum OrderTypeEnum
{
    OrderBy = 1,
    OrderByDescending = 2,
    ThenBy = 3,
    ThenByDescending = 4
}