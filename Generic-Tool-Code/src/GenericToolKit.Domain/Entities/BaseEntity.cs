namespace GenericToolKit.Domain.Entities
{

    public abstract class BaseEntity
    {

        public int Id { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? TenantId { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public int? DeletedBy { get; set; }

        // Sets deleted properties
        public void SetDeletedProperties(int loginId)
        {
            IsDeleted = true;
            DeletedOn = DateTime.Now;
            DeletedBy = loginId;
        }

        // Sets deleted properties to null
        public void SetDeletedPropertiesToNull()
        {
            IsDeleted = false;
            DeletedOn = null;
            DeletedBy = null;
        }
    }
}

