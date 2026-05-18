using GenericToolKit.Domain.Interfaces;

namespace Patient.API.Infrastructure.LoggedInUser;

public class HttpContextLoggedInUser : ILoggedInUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextLoggedInUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int TenantId
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return 1;

            if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantIdHeader))
            {
                if (int.TryParse(tenantIdHeader, out var tenantId))
                {
                    return tenantId;
                }
            }

            var tenantClaim = context.User?.FindFirst("TenantId");
            if (tenantClaim != null && int.TryParse(tenantClaim.Value, out var tenantIdFromClaim))
            {
                return tenantIdFromClaim;
            }

            return 1;
        }
        set { }
    }

    public int LoginId
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return 0;

            if (context.Request.Headers.TryGetValue("X-User-Id", out var userIdHeader))
            {
                if (int.TryParse(userIdHeader, out var userId))
                {
                    return userId;
                }
            }

            var userClaim = context.User?.FindFirst("UserId") ?? context.User?.FindFirst("sub");
            if (userClaim != null && int.TryParse(userClaim.Value, out var userIdFromClaim))
            {
                return userIdFromClaim;
            }

            return 0;
        }
        set { }
    }

    public int RoleId
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return 0;

            if (context.Request.Headers.TryGetValue("X-Role-Id", out var roleIdHeader))
            {
                if (int.TryParse(roleIdHeader, out var roleId))
                {
                    return roleId;
                }
            }

            var roleClaim = context.User?.FindFirst("RoleId") ?? context.User?.FindFirst("role");
            if (roleClaim != null && int.TryParse(roleClaim.Value, out var roleIdFromClaim))
            {
                return roleIdFromClaim;
            }

            return 0;
        }
        set { }
    }
}

