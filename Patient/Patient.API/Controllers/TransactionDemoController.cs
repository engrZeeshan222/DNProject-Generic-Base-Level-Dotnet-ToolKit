using Microsoft.AspNetCore.Mvc;
using Patient.Application.DTOs;
using Patient.Application.Services;

namespace Patient.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionDemoController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly ILogger<TransactionDemoController> _logger;

    public TransactionDemoController(
        IPatientService patientService,
        ILogger<TransactionDemoController> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    [HttpPost("create-batch")]
    [ProducesResponseType(typeof(List<PatientDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // Creates patients in transaction
    public async Task<IActionResult> CreatePatientsInTransaction(
        [FromBody] List<CreatePatientRequest> requests,
        CancellationToken cancellationToken)
    {

        var transaction = await _patientService.StartTransaction();
        var createdPatients = new List<PatientDto>();

        try
        {
            foreach (var request in requests)
            {
                var patient = await _patientService.CreatePatientAsync(request, cancellationToken);
                createdPatients.Add(patient);
            }

            await _patientService.CommitTransactionAsync(transaction, shouldCommit: true);

            return Ok(new
            {
                message = $"Successfully created {createdPatients.Count} patients in transaction",
                patients = createdPatients
            });
        }
        catch (Exception ex)
        {

            await _patientService.RollbackTransactionAsync(transaction);

            _logger.LogError(ex, "Transaction failed, rolled back");

            return BadRequest(new
            {
                error = "Transaction failed and was rolled back",
                details = ex.Message
            });
        }
    }

    [HttpPost("demo-rollback")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    // Demo rollback
    public async Task<IActionResult> DemoRollback(
        [FromBody] CreatePatientRequest request,
        CancellationToken cancellationToken)
    {
        var transaction = await _patientService.StartTransaction();

        try
        {
            var patient = await _patientService.CreatePatientAsync(request, cancellationToken);

            await _patientService.RollbackTransactionAsync(transaction);

            return Ok(new
            {
                message = "Patient was created but transaction was rolled back (demo)",
                patientId = patient.Id,
                note = "This patient will NOT exist in the database"
            });
        }
        catch (Exception ex)
        {
            await _patientService.RollbackTransactionAsync(transaction);
            return BadRequest(new { error = ex.Message });
        }
    }
}

