using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrewRedTestAssessment.Migrations
{
    public partial class AddedIsDuplicated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_trip_duplicate_detection",
                table: "Trips",
                columns: new[] { "PickupDateTime", "DropoffDateTime", "PassengerCount" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_trip_duplicate_detection",
                table: "Trips");
        }
    }
}
