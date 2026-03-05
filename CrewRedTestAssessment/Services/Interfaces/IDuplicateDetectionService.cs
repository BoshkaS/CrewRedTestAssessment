using CrewRedTestAssessment.Models.Entities;

namespace CrewRedTestAssessment.Services.Interfaces
{
    public interface IDuplicateDetectionService
    {
        Task<bool> IsDuplicateAsync(Trip trip);
        Task<List<Trip>> GetDuplicatesAsync(List<Trip> tripsToCheck);
        void ClearCache();
    }
}