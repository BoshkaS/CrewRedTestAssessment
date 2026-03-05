using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CrewRedTestAssessment.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PickupDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DropoffDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PassengerCount = table.Column<int>(type: "integer", nullable: false),
                    TripDistance = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    StoreAndFwdFlag = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    PULocationId = table.Column<int>(type: "integer", nullable: false),
                    DOLocationId = table.Column<int>(type: "integer", nullable: false),
                    FareAmount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    TipAmount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    ImportedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_trip_pulocationid",
                table: "Trips",
                column: "PULocationId");

            migrationBuilder.CreateIndex(
                name: "ix_trip_pulocationid_tipamount",
                table: "Trips",
                columns: new[] { "PULocationId", "TipAmount" });

            migrationBuilder.CreateIndex(
                name: "ix_trip_pulocationid_tripdistance",
                table: "Trips",
                columns: new[] { "PULocationId", "TripDistance" });

            migrationBuilder.CreateIndex(
                name: "ix_trip_tripdistance",
                table: "Trips",
                column: "TripDistance");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trips");
        }
    }
}
