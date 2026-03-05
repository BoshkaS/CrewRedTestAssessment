using CrewRedTestAssessment.Data;
using CrewRedTestAssessment.Models.Entities;
using CrewRedTestAssessment.Models.Mappings;
using CrewRedTestAssessment.Models.Requests;
using CrewRedTestAssessment.Models.Responses;
using CrewRedTestAssessment.Services.Interfaces;
using CsvHelper;
using EFCore.BulkExtensions;
using System.Globalization;

namespace CrewRedTestAssessment.Services
{
    public class TripImportService : ITripImportService
    {
        private readonly ICsvParsingService _csvParsingService;
        private readonly IDataTransformationService _transformationService;
        private readonly IDuplicateDetectionService _duplicateService;
        private readonly AppDbContext _dbContext;
        private readonly ILogger<TripImportService> _logger;

        public TripImportService(
            ICsvParsingService csvParsingService,
            IDataTransformationService transformationService,
            IDuplicateDetectionService duplicateService,
            AppDbContext dbContext,
            ILogger<TripImportService> logger)
        {
            _csvParsingService = csvParsingService;
            _transformationService = transformationService;
            _duplicateService = duplicateService;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ImportTripsResponse> ImportTripsAsync(ImportTripsRequest request, CancellationToken cancellationToken = default)
        {
            var startTime = DateTime.UtcNow;
            var response = new ImportTripsResponse();

            try
            {
                _logger.LogInformation("Starting trip import from: {CsvFilePath}", request.CsvFilePath);

                int totalRecordsRead = 0;
                int validRecords = 0;
                int duplicatesFound = 0;
                int validationErrors = 0;
                var batch = new List<Trip>();
                var duplicateTrips = new List<Trip>();

                await foreach (var csvRecord in _csvParsingService.ParseCsvAsync(request.CsvFilePath, cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    totalRecordsRead++;

                    var errors = new List<string>();
                    var trip = _transformationService.TransformToTrip(csvRecord, errors);

                    if (trip == null)
                    {
                        validationErrors++;
                        response.ErrorDetails.AddRange(errors);
                        continue;
                    }

                    if (!request.SkipDuplicateDetection && await _duplicateService.IsDuplicateAsync(trip))
                    {
                        duplicatesFound++;
                        duplicateTrips.Add(trip);
                        continue;
                    }

                    batch.Add(trip);
                    validRecords++;

                    if (batch.Count >= request.BatchSize)
                    {
                        await BulkInsertBatchAsync(batch, cancellationToken);
                        _logger.LogInformation("Inserted batch of {BatchSize} records. Total processed: {Total}", batch.Count, validRecords);
                        batch.Clear();
                    }
                }

                if (batch.Count > 0)
                {
                    await BulkInsertBatchAsync(batch, cancellationToken);
                    _logger.LogInformation("Inserted final batch of {BatchSize} records", batch.Count);
                }

                if (!string.IsNullOrEmpty(request.DuplicatesOutputPath) && duplicateTrips.Count > 0)
                {
                    await WriteDuplicatesToCsvAsync(duplicateTrips, request.DuplicatesOutputPath, cancellationToken);
                }

                var duration = DateTime.UtcNow - startTime;

                response.Success = true;
                response.TotalRecordsRead = totalRecordsRead;
                response.RecordsInserted = validRecords;
                response.DuplicatesFound = duplicatesFound;
                response.ValidationErrors = validationErrors;
                response.Duration = duration;
                response.Message = $"Import completed successfully. Inserted {validRecords} records, skipped {duplicatesFound} duplicates and {validationErrors} validation errors.";

                _logger.LogInformation(response.Message);
                _logger.LogInformation("Import completed in {Duration:hh\\:mm\\:ss}", duration);

                return response;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex, "Trip import was cancelled");
                response.Success = false;
                response.Message = "Import was cancelled";
                response.ErrorDetails.Add(ex.ToString());
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during trip import");
                response.Success = false;
                response.Message = $"Import failed: {ex.Message}";
                response.ErrorDetails.Add(ex.ToString());
                return response;
            }
        }

        private async Task BulkInsertBatchAsync(List<Trip> batch, CancellationToken cancellationToken)
        {
            try
            {
                if (batch.Count == 0)
                    return;

                await _dbContext.BulkInsertAsync(batch, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk insert of {Count} records", batch.Count);
                throw;
            }
        }

        private async Task WriteDuplicatesToCsvAsync(List<Trip> duplicates, string outputPath, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Writing {Count} duplicate records to: {OutputPath}", duplicates.Count, outputPath);

                using (var writer = new StreamWriter(outputPath))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteField("PickupDateTime");
                    csv.WriteField("DropoffDateTime");
                    csv.WriteField("PassengerCount");
                    csv.WriteField("TripDistance");
                    csv.WriteField("StoreAndFwdFlag");
                    csv.WriteField("PULocationId");
                    csv.WriteField("DOLocationId");
                    csv.WriteField("FareAmount");
                    csv.WriteField("TipAmount");
                    await csv.NextRecordAsync();

                    foreach (var trip in duplicates)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        csv.WriteField(trip.PickupDateTime);
                        csv.WriteField(trip.DropoffDateTime);
                        csv.WriteField(trip.PassengerCount);
                        csv.WriteField(trip.TripDistance);
                        csv.WriteField(trip.StoreAndFwdFlag);
                        csv.WriteField(trip.PULocationId);
                        csv.WriteField(trip.DOLocationId);
                        csv.WriteField(trip.FareAmount);
                        csv.WriteField(trip.TipAmount);
                        await csv.NextRecordAsync();
                    }

                    await csv.FlushAsync();
                }

                _logger.LogInformation("Successfully wrote {Count} duplicates to: {OutputPath}", duplicates.Count, outputPath);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex, "Writing duplicates to CSV was cancelled: {OutputPath}", outputPath);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing duplicates to CSV: {OutputPath}", outputPath);
                throw;
            }
        }
    }
}
