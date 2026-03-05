using CrewRedTestAssessment.Data;
using CrewRedTestAssessment.Models.Entities;
using CrewRedTestAssessment.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrewRedTestAssessment.Services
{
    public class DuplicateDetectionService : IDuplicateDetectionService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<DuplicateDetectionService> _logger;

        public DuplicateDetectionService(AppDbContext dbContext, ILogger<DuplicateDetectionService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> IsDuplicateAsync(Trip trip)
        {
            try
            {
                var isDuplicate = await _dbContext.Trips
                    .AsNoTracking()
                    .AnyAsync(t => 
                        t.PickupDateTime == trip.PickupDateTime &&
                        t.DropoffDateTime == trip.DropoffDateTime &&
                        t.PassengerCount == trip.PassengerCount);

                return isDuplicate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for duplicate trip");
                throw;
            }
        }

        public async Task<List<Trip>> GetDuplicatesAsync(List<Trip> tripsToCheck)
        {
            try
            {
                var duplicates = new List<Trip>();

                foreach (var trip in tripsToCheck)
                {
                    var duplicateInDb = await _dbContext.Trips
                        .AsNoTracking()
                        .FirstOrDefaultAsync(t =>
                            t.PickupDateTime == trip.PickupDateTime &&
                            t.DropoffDateTime == trip.DropoffDateTime &&
                            t.PassengerCount == trip.PassengerCount);

                    if (duplicateInDb != null)
                    {
                        duplicates.Add(trip);
                    }
                }

                return duplicates;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving duplicates from database");
                throw;
            }
        }
        public void ClearCache()
        {
            _logger.LogInformation("Duplicate detection using database queries (no in-memory cache to clear)");
        }
    }
}
