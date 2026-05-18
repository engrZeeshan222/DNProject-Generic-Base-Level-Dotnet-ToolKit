using GenericToolKit.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Patient.Infra.Data;

public class PatientDbContextFactory : IDesignTimeDbContextFactory<PatientDbContext>
{
    // Creates db context
    public PatientDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PatientDbContext>();

        optionsBuilder.UseSqlServer(
            @"Server=.\SQLEXPRESS;Database=PatientMicroserviceDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");

        var designTimeUser = new DesignTimeLoggedInUser();

        return new PatientDbContext(optionsBuilder.Options, designTimeUser);
    }

    private class DesignTimeLoggedInUser : ILoggedInUser
    {
        public int TenantId { get; set; } = 1;
        public int LoginId { get; set; } = 0;
        public int RoleId { get; set; } = 0;
    }
}

