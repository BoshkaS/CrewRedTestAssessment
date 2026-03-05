using CrewRedTestAssessment.Models.DTOs;

namespace CrewRedTestAssessment.Services.Interfaces
{
    public interface ICsvParsingService
    {
        IAsyncEnumerable<TripImportDto> ParseCsvAsync(string filePath, CancellationToken cancellationToken = default);
    }
}
