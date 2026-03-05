using CrewRedTestAssessment.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrewRedTestAssessment.Controllers
{
    // For testing purposes
    [ApiController]
    [Route("api/[controller]")]
    public class TripsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<TripsController> _logger;

        public TripsController(AppDbContext dbContext, ILogger<TripsController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("highest-average-tip-by-location")]
        public async Task<ActionResult<object>> GetHighestAverageTipByLocation()
        {
            try
            {
                var result = await _dbContext.Trips
                    .GroupBy(t => t.PULocationId)
                    .Select(g => new
                    {
                        PULocationId = g.Key,
                        AverageTipAmount = g.Average(t => t.TipAmount),
                        TripCount = g.Count()
                    })
                    .OrderByDescending(x => x.AverageTipAmount)
                    .FirstOrDefaultAsync();

                if (result == null)
                {
                    return NotFound(new { message = "No trip data found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving highest average tip by location");
                return StatusCode(500, new { error = "An error occurred while processing the request" });
            }
        }

        [HttpGet("top-100-longest-by-distance")]
        public async Task<ActionResult<object>> GetTop100LongestByDistance()
        {
            try
            {
                var result = await _dbContext.Trips
                    .OrderByDescending(t => t.TripDistance)
                    .Take(100)
                    .Select(t => new
                    {
                        t.Id,
                        t.PickupDateTime,
                        t.DropoffDateTime,
                        t.PassengerCount,
                        t.TripDistance,
                        t.FareAmount,
                        t.TipAmount,
                        t.PULocationId,
                        t.DOLocationId
                    })
                    .ToListAsync();

                if (!result.Any())
                {
                    return NotFound(new { message = "No trip data found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving top 100 longest fares by distance");
                return StatusCode(500, new { error = "An error occurred while processing the request" });
            }
        }

        [HttpGet("top-100-longest-by-duration")]
        public async Task<ActionResult<object>> GetTop100LongestByDuration()
        {
            try
            {
                var result = await _dbContext.Trips
                    .OrderByDescending(t => t.DropoffDateTime - t.PickupDateTime)
                    .Take(100)
                    .Select(t => new
                    {
                        t.Id,
                        t.PickupDateTime,
                        t.DropoffDateTime,
                        DurationSeconds = (long)(t.DropoffDateTime - t.PickupDateTime).TotalSeconds,
                        t.PassengerCount,
                        t.TripDistance,
                        t.FareAmount,
                        t.TipAmount,
                        t.PULocationId,
                        t.DOLocationId
                    })
                    .ToListAsync();

                if (!result.Any())
                {
                    return NotFound(new { message = "No trip data found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving top 100 longest fares by duration");
                return StatusCode(500, new { error = "An error occurred while processing the request" });
            }
        }

        [HttpGet("search-by-location")]
        public async Task<ActionResult<object>> SearchByPULocation([FromQuery] int puLocationId)
        {
            try
            {
                var trips = await _dbContext.Trips
                    .Where(t => t.PULocationId == puLocationId)
                    .OrderByDescending(t => t.PickupDateTime)
                    .Select(t => new
                    {
                        t.Id,
                        t.PickupDateTime,
                        t.DropoffDateTime,
                        t.PassengerCount,
                        t.TripDistance,
                        t.FareAmount,
                        t.TipAmount,
                        t.StoreAndFwdFlag,
                        t.DOLocationId,
                        t.ImportedDateTime
                    })
                    .ToListAsync();

                if (!trips.Any())
                {
                    return NotFound(new { message = $"No trips found for PULocationId: {puLocationId}" });
                }

                return Ok(new
                {
                    PULocationId = puLocationId,
                    TotalRecords = trips.Count,
                    Trips = trips
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching trips by location");
                return StatusCode(500, new { error = "An error occurred while processing the request" });
            }
        }
    }
}
