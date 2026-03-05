namespace CrewRedTestAssessment.Models.DTOs
{
    public class TripImportDto
    {
        public int? VendorId { get; set; }
        public string? TpepPickupDatetime { get; set; }
        public string? TpepDropoffDatetime { get; set; }
        public int? PassengerCount { get; set; }
        public decimal? TripDistance { get; set; }
        public int? RatecodeId { get; set; }
        public string? StoreAndFwdFlag { get; set; }
        public int? PULocationId { get; set; }
        public int? DOLocationId { get; set; }
        public int? PaymentType { get; set; }
        public decimal? FareAmount { get; set; }
        public decimal? Extra { get; set; }
        public decimal? MtaTax { get; set; }
        public decimal? TipAmount { get; set; }
        public decimal? TollsAmount { get; set; }
        public decimal? ImprovementSurcharge { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? CongestionSurcharge { get; set; }
    }
}
