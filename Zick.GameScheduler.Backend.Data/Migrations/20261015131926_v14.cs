using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zick.GameScheduler.Backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class v14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServerPassword",
                table: "PortClaims",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServerPassword",
                table: "PortClaims");
        }
    }
}
