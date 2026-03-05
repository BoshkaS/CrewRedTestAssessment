using CrewRedTestAssessment.Models.DTOs;
using CrewRedTestAssessment.Models.Entities;
using CrewRedTestAssessment.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace CrewRedTestAssessment.Services
{
    public class DataTransformationService : IDataTransformationService
    {
        private readonly ILogger<DataTransformationService> _logger;
        private static readonly TimeZoneInfo EstTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        private const int MaxPassengerCount = 9;
        private const int MaxLocationId = 999;

        public DataTransformationService(ILogger<DataTransformationService> logger)
        {
            _logger = logger;
        }

        public Trip? TransformToTrip(TripImportDto dto, List<string> errorMessages)
        {
            if (errorMessages == null)
            {
                throw new ArgumentNullException(nameof(errorMessages));
            }

            try
            {
                if (string.IsNullOrWhiteSpace(dto.TpepPickupDatetime))
                {
                    errorMessages.Add("tpep_pickup_datetime is required");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(dto.TpepDropoffDatetime))
                {
                    errorMessages.Add("tpep_dropoff_datetime is required");
                    return null;
                }

                if (!dto.PassengerCount.HasValue)
                {
                    errorMessages.Add("passenger_count is required");
                    return null;
                }

                if (dto.PassengerCount <= 0 || dto.PassengerCount > MaxPassengerCount)
                {
                    errorMessages.Add($"passenger_count must be between 1 and {MaxPassengerCount}");
                    return null;
                }

                if (!dto.TripDistance.HasValue)
                {
                    errorMessages.Add("trip_distance is required");
                    return null;
                }

                if (dto.TripDistance < 0)
                {
                    errorMessages.Add("trip_distance must be >= 0");
                    return null;
                }

                if (!dto.PULocationId.HasValue)
                {
                    errorMessages.Add("PULocationID is required");
                    return null;
                }

                if (dto.PULocationId <= 0 || dto.PULocationId > MaxLocationId)
                {
                    errorMessages.Add($"PULocationID must be between 1 and {MaxLocationId}");
                    return null;
                }

                if (!dto.DOLocationId.HasValue)
                {
                    errorMessages.Add("DOLocationID is required");
                    return null;
                }

                if (dto.DOLocationId <= 0 || dto.DOLocationId > MaxLocationId)
                {
                    errorMessages.Add($"DOLocationID must be between 1 and {MaxLocationId}");
                    return null;
                }

                if (!dto.FareAmount.HasValue)
                {
                    errorMessages.Add("fare_amount is required");
                    return null;
                }

                if (dto.FareAmount < 0)
                {
                    errorMessages.Add("fare_amount must be >= 0");
                    return null;
                }

                if (!dto.TipAmount.HasValue)
                {
                    errorMessages.Add("tip_amount is required");
                    return null;
                }

                if (dto.TipAmount < 0)
                {
                    errorMessages.Add("tip_amount must be >= 0");
                    return null;
                }

                if (!DateTime.TryParse(dto.TpepPickupDatetime, out var pickupLocalTime))
                {
                    errorMessages.Add($"Invalid pickup datetime format: {dto.TpepPickupDatetime}");
                    return null;
                }

                if (!DateTime.TryParse(dto.TpepDropoffDatetime, out var dropoffLocalTime))
                {
                    errorMessages.Add($"Invalid dropoff datetime format: {dto.TpepDropoffDatetime}");
                    return null;
                }

                var pickupUtc = TimeZoneInfo.ConvertTimeToUtc(pickupLocalTime, EstTimeZone);
                var dropoffUtc = TimeZoneInfo.ConvertTimeToUtc(dropoffLocalTime, EstTimeZone);

                if (dropoffUtc <= pickupUtc)
                {
                    errorMessages.Add("Dropoff time must be after pickup time");
                    return null;
                }


                var storeAndFwdFlag = NormalizeStoreAndFwdFlag(dto.StoreAndFwdFlag);

                var trip = new Trip
                {
                    PickupDateTime = pickupUtc,
                    DropoffDateTime = dropoffUtc,
                    PassengerCount = dto.PassengerCount.Value,
                    TripDistance = dto.TripDistance.Value,
                    StoreAndFwdFlag = storeAndFwdFlag,
                    PULocationId = dto.PULocationId.Value,
                    DOLocationId = dto.DOLocationId.Value,
                    FareAmount = dto.FareAmount.Value,
                    TipAmount = dto.TipAmount.Value,
                    ImportedDateTime = DateTime.UtcNow
                };

                return trip;
            }
            catch (Exception ex)
            {
                errorMessages.Add($"Error transforming record: {ex.Message}");
                _logger.LogWarning(ex, "Error transforming trip record");
                return null;
            }
        }

        private string NormalizeStoreAndFwdFlag(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "No";
            }

            return value.Trim().ToUpperInvariant() switch
            {
                "Y" => "Yes",
                "N" => "No",
                "YES" => "Yes",
                "NO" => "No",
                _ => value.Trim()
            };
        }

        public string GenerateRecordHash(Trip trip)
        {
            var hashString = $"{trip.PickupDateTime:O}|{trip.DropoffDateTime:O}|{trip.PassengerCount}";

            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(hashString));
                return Convert.ToHexString(hashedBytes);
            }
        }
    }
}
