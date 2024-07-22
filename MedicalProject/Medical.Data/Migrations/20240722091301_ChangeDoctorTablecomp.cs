using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Medical.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDoctorTablecomp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Compassion",
                table: "Doctors",
                newName: "CompassionSkil");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompassionSkil",
                table: "Doctors",
                newName: "Compassion");
        }
    }
}
