using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zick.GameScheduler.Backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class v6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PortClaimId",
                table: "Sessions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PortClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClaimedPort = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimedHttpPort = table.Column<string>(type: "TEXT", nullable: false),
                    IsInUse = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortClaims", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_PortClaimId",
                table: "Sessions",
                column: "PortClaimId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_PortClaims_PortClaimId",
                table: "Sessions",
                column: "PortClaimId",
                principalTable: "PortClaims",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_PortClaims_PortClaimId",
                table: "Sessions");

            migrationBuilder.DropTable(
                name: "PortClaims");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_PortClaimId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "PortClaimId",
                table: "Sessions");
        }
    }
}
