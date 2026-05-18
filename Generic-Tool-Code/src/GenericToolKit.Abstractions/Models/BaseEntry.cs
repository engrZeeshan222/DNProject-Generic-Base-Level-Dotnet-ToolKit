namespace GenericToolKit.Domain.Models
{

    public class BaseEntry<T>
    {

        public TrackedEntityState State { get; set; }

        public object? CurrentValues { get; set; }

        public object? OriginalValues { get; set; }

        public T? Entity { get; set; }

        public Dictionary<string, object> ModifiedProperties { get; set; } = new();
    }
}

