using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MEDCHGClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class added_columns_in_patientvisithistory_and_patientvisithistorydetaillist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecordStatus",
                table: "PatientVisitHistoryDetailLists",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientBilledById",
                table: "PatientVisitHistory",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PatientBilledDate",
                table: "PatientVisitHistory",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientCensusStatus",
                table: "PatientVisitHistory",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientDischargeById",
                table: "PatientVisitHistory",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PatientDischargeDate",
                table: "PatientVisitHistory",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecordStatus",
                table: "PatientVisitHistoryDetailLists");

            migrationBuilder.DropColumn(
                name: "PatientBilledById",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "PatientBilledDate",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "PatientCensusStatus",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "PatientDischargeById",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "PatientDischargeDate",
                table: "PatientVisitHistory");
        }
    }
}
