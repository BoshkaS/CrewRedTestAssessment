using CsvHelper;
using CsvHelper.Configuration;
using CrewRedTestAssessment.Models.DTOs;
using CrewRedTestAssessment.Models.Mappings;
using CrewRedTestAssessment.Services.Interfaces;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace CrewRedTestAssessment.Services
{
    public class CsvParsingService : ICsvParsingService
    {
        private readonly ILogger<CsvParsingService> _logger;

        public CsvParsingService(ILogger<CsvParsingService> logger)
        {
            _logger = logger;
        }

        public async IAsyncEnumerable<TripImportDto> ParseCsvAsync(
            string filePath, 
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                _logger.LogError("CSV file path is null or empty");
                throw new ArgumentException("CSV file path cannot be null or empty", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                _logger.LogError("CSV file not found: {FilePath}", filePath);
                throw new FileNotFoundException($"CSV file not found: {filePath}");
            }

            _logger.LogInformation("Starting to parse CSV file: {FilePath}", filePath);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                BadDataFound = null,
                MissingFieldFound = null,
                IgnoreBlankLines = true,
            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<TripImportDtoMap>();

                var recordCount = 0;

                await foreach (var record in csv.GetRecordsAsync<TripImportDto>(cancellationToken).ConfigureAwait(false))
                {
                    recordCount++;
                    yield return record;

                    if (recordCount % 1000 == 0)
                    {
                        _logger.LogInformation("Parsed {RecordCount} records", recordCount);
                    }
                }

                _logger.LogInformation("Finished parsing CSV. Total records: {RecordCount}", recordCount);
            }
        }
    }
}
