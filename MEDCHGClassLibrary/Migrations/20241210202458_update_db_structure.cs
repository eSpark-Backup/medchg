using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MEDCHGClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class update_db_structure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientTempCodes_PatientVisits_PatientVisitId",
                table: "PatientTempCodes");

            migrationBuilder.DropTable(
                name: "PatientChargeCodes");

            migrationBuilder.DropTable(
                name: "PatientDiagnosticCodes");

            migrationBuilder.DropTable(
                name: "PatientUsers");

            migrationBuilder.DropTable(
                name: "PatientVisits");

            migrationBuilder.DropIndex(
                name: "IX_PatientTempCodes_PatientVisitId",
                table: "PatientTempCodes");

            migrationBuilder.DropColumn(
                name: "MRNnumber",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "PatientDOB",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "PatientName",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "PatientSalutation",
                table: "PatientVisitHistory");

            migrationBuilder.RenameColumn(
                name: "PatientGender",
                table: "PatientVisitHistory",
                newName: "UserId");

            migrationBuilder.AddColumn<int>(
                name: "BuildingId",
                table: "PatientVisitHistory",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FloorId",
                table: "PatientVisitHistory",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "PatientVisitHistory",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientVisitHistoryId",
                table: "PatientTempCodes",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuildingId",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "FloorId",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "PatientVisitHistoryId",
                table: "PatientTempCodes");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PatientVisitHistory",
                newName: "PatientGender");

            migrationBuilder.AddColumn<string>(
                name: "MRNnumber",
                table: "PatientVisitHistory",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "PatientDOB",
                table: "PatientVisitHistory",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PatientName",
                table: "PatientVisitHistory",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PatientSalutation",
                table: "PatientVisitHistory",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PatientVisits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FloorId = table.Column<int>(type: "integer", nullable: true),
                    PatientId = table.Column<int>(type: "integer", nullable: false),
                    RoomId = table.Column<int>(type: "integer", nullable: true),
                    BuildingId = table.Column<int>(type: "integer", nullable: true),
                    CensusStatus = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Device = table.Column<string>(type: "text", nullable: true),
                    HospitalId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientVisits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientVisits_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PatientVisits_Floors_FloorId",
                        column: x => x.FloorId,
                        principalTable: "Floors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PatientVisits_Hospitals_HospitalId",
                        column: x => x.HospitalId,
                        principalTable: "Hospitals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientVisits_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientVisits_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PatientChargeCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChargeCode = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Device = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<int>(type: "integer", nullable: false),
                    IsCurrentlyAdd = table.Column<bool>(type: "boolean", nullable: false),
                    PatientVisitId = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientChargeCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientChargeCodes_PatientVisits_PatientVisitId",
                        column: x => x.PatientVisitId,
                        principalTable: "PatientVisits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientDiagnosticCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Device = table.Column<string>(type: "text", nullable: true),
                    DiagnosticCode = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<int>(type: "integer", nullable: false),
                    IsCurrentlyAdd = table.Column<bool>(type: "boolean", nullable: false),
                    PatientVisitId = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientDiagnosticCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientDiagnosticCodes_PatientVisits_PatientVisitId",
                        column: x => x.PatientVisitId,
                        principalTable: "PatientVisits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Device = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<int>(type: "integer", nullable: false),
                    PatientVisitId = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientUsers_PatientVisits_PatientVisitId",
                        column: x => x.PatientVisitId,
                        principalTable: "PatientVisits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientTempCodes_PatientVisitId",
                table: "PatientTempCodes",
                column: "PatientVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientChargeCodes_PatientVisitId",
                table: "PatientChargeCodes",
                column: "PatientVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDiagnosticCodes_PatientVisitId",
                table: "PatientDiagnosticCodes",
                column: "PatientVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientUsers_PatientVisitId",
                table: "PatientUsers",
                column: "PatientVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientVisits_BuildingId",
                table: "PatientVisits",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientVisits_FloorId",
                table: "PatientVisits",
                column: "FloorId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientVisits_HospitalId",
                table: "PatientVisits",
                column: "HospitalId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientVisits_PatientId",
                table: "PatientVisits",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientVisits_RoomId",
                table: "PatientVisits",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientTempCodes_PatientVisits_PatientVisitId",
                table: "PatientTempCodes",
                column: "PatientVisitId",
                principalTable: "PatientVisits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
