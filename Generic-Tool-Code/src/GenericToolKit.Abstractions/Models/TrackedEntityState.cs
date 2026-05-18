namespace GenericToolKit.Domain.Models
{

    public enum TrackedEntityState
    {
        Detached = 0,
        Unchanged = 1,
        Deleted = 2,
        Modified = 3,
        Added = 4
    }
}

