using System.Linq.Expressions;
using GenericToolKit.Domain.Interfaces;
using GenericToolKit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Patient.Domain.Entities;

namespace Patient.Infra.Data;

public class PatientDbContext : BaseContext
{
    private readonly ILoggedInUser _loggedInUser;

    public PatientDbContext(
        DbContextOptions<PatientDbContext> options,
        ILoggedInUser loggedInUser)
        : base(options, loggedInUser)
    {
        _loggedInUser = loggedInUser;
    }

    public DbSet<Domain.Entities.Patient> Patients { get; set; } = null!;
    public DbSet<Appointment> Appointments { get; set; } = null!;

    // Configures the EF Core model
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfiguration(new PatientEntityConfiguration());
        modelBuilder.ApplyConfiguration(new AppointmentEntityConfiguration());

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {

            if (typeof(GenericToolKit.Domain.Entities.BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");

                var isDeletedProperty = Expression.Property(parameter, "IsDeleted");
                var isDeletedFilter = Expression.NotEqual(
                    isDeletedProperty,
                    Expression.Constant(true, typeof(bool?))
                );

                var tenantIdProperty = Expression.Property(parameter, "TenantId");
                var currentTenantId = Expression.Constant(_loggedInUser.TenantId, typeof(int?));

                Expression combinedFilter;

                if (_loggedInUser.TenantId > 0)
                {
                    var tenantFilter = Expression.Equal(tenantIdProperty, currentTenantId);
                    combinedFilter = Expression.AndAlso(isDeletedFilter, tenantFilter);
                }
                else
                {
                    combinedFilter = isDeletedFilter;
                }

                var lambda = Expression.Lambda(combinedFilter, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}

