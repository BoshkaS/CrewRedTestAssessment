using CrewRedTestAssessment.Models.Requests;
using CrewRedTestAssessment.Models.Responses;
using CrewRedTestAssessment.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CrewRedTestAssessment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : ControllerBase
    {
        private readonly ITripImportService _tripImportService;
        private readonly ILogger<ImportController> _logger;

        public ImportController(ITripImportService tripImportService, ILogger<ImportController> logger)
        {
            _tripImportService = tripImportService;
            _logger = logger;
        }
        
        [HttpPost("trips")]
        public async Task<ActionResult<ImportTripsResponse>> ImportTrips(
            [FromBody] ImportTripsRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Import request received. CsvFilePath: '{CsvFilePath}', BatchSize: {BatchSize}, SkipDuplicates: {Skip}", 
                request?.CsvFilePath ?? "NULL", request?.BatchSize ?? 0, request?.SkipDuplicateDetection ?? false);

            if (request == null)
            {
                return BadRequest(new { error = "Request body is required" });
            }

            if (string.IsNullOrWhiteSpace(request.CsvFilePath))
            {
                return BadRequest(new { error = "CSV file path is required", receivedPath = request.CsvFilePath ?? "NULL" });
            }

            try
            {
                var response = await _tripImportService.ImportTripsAsync(request, cancellationToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during import request");
                return StatusCode(500, new { error = "An error occurred during import", details = ex.Message });
            }
        }
    }
}
