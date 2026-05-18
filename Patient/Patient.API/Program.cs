using GenericToolKit.Application.DependencyInjection;
using GenericToolKit.Domain.Interfaces;
using GenericToolKit.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Patient.API.Infrastructure.LoggedInUser;
using Patient.Application.Services;
using Patient.Infra.Data;
using Patient.Infra.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ILoggedInUser, HttpContextLoggedInUser>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PatientDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<PatientDbContext>());

builder.Services.AddGenericRepository<Patient.Domain.Entities.Patient>();
builder.Services.AddGenericRepository<Patient.Domain.Entities.Appointment>();

builder.Services.AddScoped<IPatientRepository, PatientRepository>();

builder.Services.AddGenericService<Patient.Domain.Entities.Patient>();
builder.Services.AddGenericService<Patient.Domain.Entities.Appointment>();

builder.Services.AddScoped<IPatientService, PatientService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {

        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

    var tenantId = context.Request.Headers["X-Tenant-Id"].FirstOrDefault() ?? "Not provided";
    var userId = context.Request.Headers["X-User-Id"].FirstOrDefault() ?? "Not provided";
    var roleId = context.Request.Headers["X-Role-Id"].FirstOrDefault() ?? "Not provided";

    logger.LogInformation(
        "Request: {Method} {Path} | Tenant: {TenantId} | User: {UserId} | Role: {RoleId}",
        context.Request.Method,
        context.Request.Path,
        tenantId,
        userId,
        roleId);

    await next();
});

app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            var context = services.GetRequiredService<PatientDbContext>();

            logger.LogInformation("Checking for pending database migrations...");

            if (context.Database.GetPendingMigrations().Any())
            {
                logger.LogInformation("Applying pending migrations...");
                context.Database.Migrate();
                logger.LogInformation("Migrations applied successfully");
            }
            else
            {
                logger.LogInformation("Database is up to date");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database");
            throw;
        }
    }
}

var startupLogger = app.Services.GetRequiredService<ILogger<Program>>();
startupLogger.LogInformation("Patient Microservice started successfully");
startupLogger.LogInformation("API available at: https://localhost:7001");
startupLogger.LogInformation("Use Postman collection for API testing");
startupLogger.LogInformation("\n" +
    "===========================================\n" +
    "PATIENT MICROSERVICE API\n" +
    "===========================================\n" +
    "Generic Toolkit Features Demonstrated:\n" +
    "- Multi-tenancy (X-Tenant-Id header)\n" +
    "- User tracking (X-User-Id header)\n" +
    "- Role management (X-Role-Id header)\n" +
    "- Automatic audit fields\n" +
    "- Soft delete support\n" +
    "- Repository & Service patterns\n" +
    "- Specification pattern\n" +
    "- Transaction management\n" +
    "- Change tracking\n" +
    "===========================================\n");

app.Run();

