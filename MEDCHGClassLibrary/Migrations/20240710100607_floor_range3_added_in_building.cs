using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MEDCHGClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class floor_range3_added_in_building : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FloorRange3",
                table: "Buildings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FloorRange3",
                table: "Buildings");
        }
    }
}
