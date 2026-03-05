namespace CrewRedTestAssessment.Models.Requests
{
    public class ImportTripsRequest
    {
        public string CsvFilePath { get; set; } = string.Empty;
        public string? DuplicatesOutputPath { get; set; }
        public int BatchSize { get; set; } = 10000;
        public bool SkipDuplicateDetection { get; set; } = false;
    }
}
