using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MEDCHGClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class remove_2_columns_from_patientvisithistory_and_patientvisithistorydetaillist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecordStatus",
                table: "PatientVisitHistoryDetailLists");

            migrationBuilder.DropColumn(
                name: "PatientStatus",
                table: "PatientVisitHistory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecordStatus",
                table: "PatientVisitHistoryDetailLists",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PatientStatus",
                table: "PatientVisitHistory",
                type: "text",
                nullable: true);
        }
    }
}
