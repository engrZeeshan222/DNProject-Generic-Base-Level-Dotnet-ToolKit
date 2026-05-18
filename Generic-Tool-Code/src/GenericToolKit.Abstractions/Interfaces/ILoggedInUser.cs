namespace GenericToolKit.Domain.Interfaces
{

    public interface ILoggedInUser
    {

        int TenantId { get; set; }

        int LoginId { get; set; }

        int RoleId { get; set; }
    }
}

