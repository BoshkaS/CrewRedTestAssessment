using CsvHelper.Configuration;
using CrewRedTestAssessment.Models.DTOs;

namespace CrewRedTestAssessment.Models.Mappings
{
    public sealed class TripImportDtoMap : ClassMap<TripImportDto>
    {
        public TripImportDtoMap()
        {
            Map(m => m.VendorId).Name("VendorID");
            Map(m => m.TpepPickupDatetime).Name("tpep_pickup_datetime");
            Map(m => m.TpepDropoffDatetime).Name("tpep_dropoff_datetime");
            Map(m => m.PassengerCount).Name("passenger_count");
            Map(m => m.TripDistance).Name("trip_distance");
            Map(m => m.RatecodeId).Name("RatecodeID");
            Map(m => m.StoreAndFwdFlag).Name("store_and_fwd_flag");
            Map(m => m.PULocationId).Name("PULocationID");
            Map(m => m.DOLocationId).Name("DOLocationID");
            Map(m => m.PaymentType).Name("payment_type");
            Map(m => m.FareAmount).Name("fare_amount");
            Map(m => m.Extra).Name("extra");
            Map(m => m.MtaTax).Name("mta_tax");
            Map(m => m.TipAmount).Name("tip_amount");
            Map(m => m.TollsAmount).Name("tolls_amount");
            Map(m => m.ImprovementSurcharge).Name("improvement_surcharge");
            Map(m => m.TotalAmount).Name("total_amount");
            Map(m => m.CongestionSurcharge).Name("congestion_surcharge");
        }
    }
}
