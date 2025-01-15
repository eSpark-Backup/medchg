using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MEDCHGClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class add_two_tables_patientvisithistorydetaillist_providertimersetting_and_remove_foreign_keys_from_patientvisithistory_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientVisitHistory_PatientVisits_PatientVisitId",
                table: "PatientVisitHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientVisitHistory_Patients_PatientId",
                table: "PatientVisitHistory");

            migrationBuilder.DropIndex(
                name: "IX_PatientVisitHistory_PatientId",
                table: "PatientVisitHistory");

            migrationBuilder.DropIndex(
                name: "IX_PatientVisitHistory_PatientVisitId",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "PatientVisitId",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "PatientVisitHistory");

            migrationBuilder.AlterColumn<int>(
                name: "PatientId",
                table: "PatientVisitHistory",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "MRNnumber",
                table: "PatientVisitHistory",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PatientAddmissionDate",
                table: "PatientVisitHistory",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PatientAddmissionTime",
                table: "PatientVisitHistory",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientAge",
                table: "PatientVisitHistory",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PatientDOB",
                table: "PatientVisitHistory",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientGender",
                table: "PatientVisitHistory",
                type: "integer",
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

            migrationBuilder.AddColumn<string>(
                name: "PatientStatus",
                table: "PatientVisitHistory",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PatientVisitHistoryDetailLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodeName = table.Column<string>(type: "text", nullable: true),
                    CodeDescription = table.Column<string>(type: "text", nullable: true),
                    CodeType = table.Column<string>(type: "text", nullable: true),
                    CodeSubType = table.Column<string>(type: "text", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    PractitionerName = table.Column<string>(type: "text", nullable: true),
                    RoomName = table.Column<string>(type: "text", nullable: true),
                    FloorName = table.Column<string>(type: "text", nullable: true),
                    BuildingName = table.Column<string>(type: "text", nullable: true),
                    HospitalName = table.Column<string>(type: "text", nullable: true),
                    RoomId = table.Column<int>(type: "integer", nullable: true),
                    FloorId = table.Column<int>(type: "integer", nullable: true),
                    BuildingId = table.Column<int>(type: "integer", nullable: true),
                    HospitalId = table.Column<int>(type: "integer", nullable: true),
                    PractitionerId = table.Column<int>(type: "integer", nullable: true),
                    PatientVisitHistoryId = table.Column<int>(type: "integer", nullable: true),
                    CensusState = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    Device = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientVisitHistoryDetailLists", x => x.Id);
                });

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientVisitHistoryDetailLists");

            migrationBuilder.DropColumn(
                name: "MRNnumber",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "PatientAddmissionDate",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "PatientAddmissionTime",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "PatientAge",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "PatientDOB",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "PatientGender",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "PatientName",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "PatientSalutation",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "PatientStatus",
                table: "PatientVisitHistory");

            migrationBuilder.AlterColumn<int>(
                name: "PatientId",
                table: "PatientVisitHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PatientVisitHistory",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PatientVisitId",
                table: "PatientVisitHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "PatientVisitHistory",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "PatientVisitHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "PatientVisitHistory",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PatientVisitHistory_PatientId",
                table: "PatientVisitHistory",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientVisitHistory_PatientVisitId",
                table: "PatientVisitHistory",
                column: "PatientVisitId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientVisitHistory_PatientVisits_PatientVisitId",
                table: "PatientVisitHistory",
                column: "PatientVisitId",
                principalTable: "PatientVisits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientVisitHistory_Patients_PatientId",
                table: "PatientVisitHistory",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
