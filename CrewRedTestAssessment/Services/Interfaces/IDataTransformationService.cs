using CrewRedTestAssessment.Models.DTOs;
using CrewRedTestAssessment.Models.Entities;

namespace CrewRedTestAssessment.Services.Interfaces
{
    public interface IDataTransformationService
    {
        Trip? TransformToTrip(TripImportDto dto, List<string> errorMessages);
        string GenerateRecordHash(Trip trip);
    }
}
