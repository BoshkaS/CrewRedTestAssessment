namespace CrewRedTestAssessment.Models.Entities
{
    public class Trip
    {
        public long Id { get; set; }
        public DateTime PickupDateTime { get; set; }
        public DateTime DropoffDateTime { get; set; }
        public int PassengerCount { get; set; }
        public decimal TripDistance { get; set; }
        public string StoreAndFwdFlag { get; set; } = string.Empty;
        public int PULocationId { get; set; }
        public int DOLocationId { get; set; }
        public decimal FareAmount { get; set; }
        public decimal TipAmount { get; set; }
        public DateTime ImportedDateTime { get; set; }
    }
}
