using GenericToolKit.Domain.Entities;
using GenericToolKit.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace GenericToolKit.Infrastructure.Data
{

    public abstract class BaseContext : DbContext
    {
        private ILoggedInUser? _loggedInUser;

        protected BaseContext(DbContextOptions options, ILoggedInUser? loggedInUser = null)
            : base(options)
        {
            _loggedInUser = loggedInUser;
        }

        public ILoggedInUser? loggedInUser
        {
            get => _loggedInUser;
            set => _loggedInUser = value;
        }

        // Configures the EF Core model
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureBaseEntityProperties(modelBuilder);
        }

        // Configure global filters for all entities inheriting from BaseEntity
        private void ConfigureBaseEntityProperties(ModelBuilder modelBuilder)
        {
            // Get all entities that inherit from BaseEntity
            var entityTypes = modelBuilder.Model.GetEntityTypes()
                .Where(x => typeof(BaseEntity).IsAssignableFrom(x.ClrType));

            foreach (var entityType in entityTypes)
            {
                // Example generated parameter:
                // e =>
                var parameter = Expression.Parameter(entityType.ClrType, "e");

                // Create filter:
                // e.IsDeleted == false
                var isDeletedFilter = Expression.Equal(
                    Expression.Property(parameter, nameof(BaseEntity.IsDeleted)),
                    Expression.Constant(false)
                );

                Expression finalFilter = isDeletedFilter;

                // Add tenant filter if logged-in user exists
                if (_loggedInUser != null)
                {
                    // e.TenantId == currentTenantId
                    var tenantFilter = Expression.Equal(
                        Expression.Property(parameter, nameof(BaseEntity.TenantId)),
                        Expression.Constant(_loggedInUser.TenantId)
                    );

                    // Combine both filters:
                    // e.IsDeleted == false && e.TenantId == currentTenantId
                    finalFilter = Expression.AndAlso(isDeletedFilter, tenantFilter);
                }

                // Convert expression into lambda:
                // e => e.IsDeleted == false && e.TenantId == currentTenantId
                var lambda = Expression.Lambda(finalFilter, parameter);

                // Apply global query filter
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(lambda);
            }
        }

        // Saves or updates changes
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetAuditProperties();
            return await base.SaveChangesAsync(cancellationToken);
        }

        // Saves or updates changes
        public override int SaveChanges()
        {
            SetAuditProperties();
            return base.SaveChanges();
        }

        // Sets audit properties
        private void SetAuditProperties()
        {
            if (_loggedInUser == null)
                return;

            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            var timestamp = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedBy = _loggedInUser.LoginId;
                    entity.CreatedOn = timestamp;
                }

                entity.UpdatedBy = _loggedInUser.LoginId;
                entity.UpdatedOn = timestamp;
                entity.TenantId = _loggedInUser.TenantId;
                entity.IsDeleted = entity.IsDeleted ?? false;
            }
        }
    }
}

