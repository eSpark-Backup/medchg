using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MEDCHGClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class add_column_in_patient_as_patientadmissiondatetime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PatientAddmissionDateTime",
                table: "Patients",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatientAddmissionDateTime",
                table: "Patients");
        }
    }
}
