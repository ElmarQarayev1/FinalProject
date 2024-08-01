using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Medical.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedresetpassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPasswordResetRequired",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPasswordResetRequired",
                table: "AspNetUsers");
        }
    }
}
