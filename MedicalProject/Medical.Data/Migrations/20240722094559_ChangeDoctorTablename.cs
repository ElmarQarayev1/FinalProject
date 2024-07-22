using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Medical.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDoctorTablename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Doctors",
                newName: "FullName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Doctors",
                newName: "Name");
        }
    }
}
