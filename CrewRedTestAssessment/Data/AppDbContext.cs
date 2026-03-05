using CrewRedTestAssessment.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrewRedTestAssessment.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Trip> Trips { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var tripEntity = modelBuilder.Entity<Trip>();

            tripEntity.HasKey(t => t.Id);
            tripEntity.Property(t => t.Id).ValueGeneratedOnAdd();
            tripEntity.Property(t => t.PickupDateTime)
                .HasColumnType("timestamp with time zone")
                .IsRequired();
            tripEntity.Property(t => t.DropoffDateTime)
                .HasColumnType("timestamp with time zone")
                .IsRequired();
            tripEntity.Property(t => t.PassengerCount)
                .IsRequired();
            tripEntity.Property(t => t.TripDistance)
                .HasColumnType("numeric(10, 2)")
                .IsRequired();
            tripEntity.Property(t => t.StoreAndFwdFlag)
                .HasMaxLength(3)
                .IsRequired();
            tripEntity.Property(t => t.PULocationId)
                .IsRequired();
            tripEntity.Property(t => t.DOLocationId)
                .IsRequired();
            tripEntity.Property(t => t.FareAmount)
                .HasColumnType("numeric(10, 2)")
                .IsRequired();
            tripEntity.Property(t => t.TipAmount)
                .HasColumnType("numeric(10, 2)")
                .IsRequired();

            // INDEXES
            tripEntity.HasIndex(t => new { t.PickupDateTime, t.DropoffDateTime, t.PassengerCount })
                .HasDatabaseName("ix_trip_duplicate_detection");

            tripEntity.HasIndex(t => t.PULocationId)
                .HasDatabaseName("ix_trip_pulocationid");

            tripEntity.HasIndex(t => t.TripDistance)
                .HasDatabaseName("ix_trip_tripdistance");

            tripEntity.HasIndex(t => new { t.PULocationId, t.TipAmount })
                .HasDatabaseName("ix_trip_pulocationid_tipamount");

            tripEntity.HasIndex(t => new { t.PULocationId, t.TripDistance })
                .HasDatabaseName("ix_trip_pulocationid_tripdistance");

            tripEntity.ToTable("Trips");
        }
    }
}
