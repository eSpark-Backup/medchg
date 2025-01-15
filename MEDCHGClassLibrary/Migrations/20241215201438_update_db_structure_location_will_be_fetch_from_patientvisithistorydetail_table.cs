using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MEDCHGClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class update_db_structure_location_will_be_fetch_from_patientvisithistorydetail_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuildingName",
                table: "PatientVisitHistoryDetailLists");

            migrationBuilder.DropColumn(
                name: "CensusState",
                table: "PatientVisitHistoryDetailLists");

            migrationBuilder.DropColumn(
                name: "FloorName",
                table: "PatientVisitHistoryDetailLists");

            migrationBuilder.DropColumn(
                name: "HospitalName",
                table: "PatientVisitHistoryDetailLists");

            migrationBuilder.DropColumn(
                name: "RoomName",
                table: "PatientVisitHistoryDetailLists");

            migrationBuilder.DropColumn(
                name: "BuildingId",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "FloorId",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "HospitalId",
                table: "PatientVisitHistory");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "PatientVisitHistory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BuildingName",
                table: "PatientVisitHistoryDetailLists",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CensusState",
                table: "PatientVisitHistoryDetailLists",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FloorName",
                table: "PatientVisitHistoryDetailLists",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HospitalName",
                table: "PatientVisitHistoryDetailLists",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomName",
                table: "PatientVisitHistoryDetailLists",
                type: "text",
                nullable: true);

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
                name: "HospitalId",
                table: "PatientVisitHistory",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "PatientVisitHistory",
                type: "integer",
                nullable: true);
        }
    }
}
