using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MEDCHGClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class two_columns_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiagnosticCodeDescription",
                table: "DiagnosticCodes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChargeCodeDescription",
                table: "ChargeCodes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiagnosticCodeDescription",
                table: "DiagnosticCodes");

            migrationBuilder.DropColumn(
                name: "ChargeCodeDescription",
                table: "ChargeCodes");
        }
    }
}
