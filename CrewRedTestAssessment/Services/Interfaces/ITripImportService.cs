using CrewRedTestAssessment.Models.Requests;
using CrewRedTestAssessment.Models.Responses;

namespace CrewRedTestAssessment.Services.Interfaces
{
    public interface ITripImportService
    {
        Task<ImportTripsResponse> ImportTripsAsync(ImportTripsRequest request, CancellationToken cancellationToken = default);
    }
}
