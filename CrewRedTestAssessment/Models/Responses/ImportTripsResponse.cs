namespace CrewRedTestAssessment.Models.Responses
{
    public class ImportTripsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TotalRecordsRead { get; set; }
        public int RecordsInserted { get; set; }
        public int DuplicatesFound { get; set; }
        public int ValidationErrors { get; set; }
        public TimeSpan Duration { get; set; }
        public List<string> ErrorDetails { get; set; } = new();
    }
}
