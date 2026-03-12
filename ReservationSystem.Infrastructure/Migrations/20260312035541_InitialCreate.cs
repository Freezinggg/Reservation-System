using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "seat_category",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    RemainingSeats = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seat_category", x => x.Id);
                    table.CheckConstraint("ck_seat_category_capacity_positive", "\"Capacity\" > 0");
                    table.CheckConstraint("ck_seat_category_remaining_non_negative", "\"RemainingSeats\" >= 0");
                    table.CheckConstraint("ck_seat_category_remaining_within_capacity", "\"RemainingSeats\" <= \"Capacity\"");
                    table.ForeignKey(
                        name: "FK_seat_category_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reservation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SeatCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservation", x => x.Id);
                    table.CheckConstraint("ck_reservation_expiration_valid", "\"ExpiresAt\" > \"CreatedAt\"");
                    table.CheckConstraint("ck_reservation_quantity_positive", "\"Quantity\" > 0");
                    table.CheckConstraint("ck_reservation_status_valid", "\"Status\" IN (1,2,3,4)");
                    table.ForeignKey(
                        name: "FK_reservation_seat_category_SeatCategoryId",
                        column: x => x.SeatCategoryId,
                        principalTable: "seat_category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_reservation_expiration",
                table: "reservation",
                column: "ExpiresAt",
                filter: "\"Status\" = 3");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_SeatCategoryId",
                table: "reservation",
                column: "SeatCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_seat_category_EventId_Name",
                table: "seat_category",
                columns: new[] { "EventId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reservation");

            migrationBuilder.DropTable(
                name: "seat_category");

            migrationBuilder.DropTable(
                name: "Event");
        }
    }
}
