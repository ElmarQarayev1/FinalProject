using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Medical.Data.Migrations
{
    /// <inheritdoc />
    public partial class Feedstablecreatedaddedimage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "Feeds",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "Feeds");
        }
    }
}
