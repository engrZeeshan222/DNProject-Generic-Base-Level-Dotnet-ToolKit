using GenericToolKit.Domain.Interfaces;

namespace Patient.API.Infrastructure.LoggedInUser;

public class SystemUser : ILoggedInUser
{
    public int TenantId { get; set; } = 1;
    public int LoginId { get; set; } = 0;
    public int RoleId { get; set; } = 999;
}

