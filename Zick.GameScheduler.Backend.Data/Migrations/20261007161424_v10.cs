using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zick.GameScheduler.Backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class v10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Results_Sessions_LeagueSession<RacingUserIdentity>Id",
                table: "Results");

            migrationBuilder.DropIndex(
                name: "IX_Results_LeagueSession<RacingUserIdentity>Id",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "LeagueSession<RacingUserIdentity>Id",
                table: "Results");

            migrationBuilder.AddColumn<Guid>(
                name: "SessionId",
                table: "Results",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Results_SessionId",
                table: "Results",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Results_Sessions_SessionId",
                table: "Results",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Results_Sessions_SessionId",
                table: "Results");

            migrationBuilder.DropIndex(
                name: "IX_Results_SessionId",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Results");

            migrationBuilder.AddColumn<Guid>(
                name: "LeagueSession<RacingUserIdentity>Id",
                table: "Results",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Results_LeagueSession<RacingUserIdentity>Id",
                table: "Results",
                column: "LeagueSession<RacingUserIdentity>Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Results_Sessions_LeagueSession<RacingUserIdentity>Id",
                table: "Results",
                column: "LeagueSession<RacingUserIdentity>Id",
                principalTable: "Sessions",
                principalColumn: "Id");
        }
    }
}
