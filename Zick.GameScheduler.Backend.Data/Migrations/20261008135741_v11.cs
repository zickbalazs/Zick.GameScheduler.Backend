using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zick.GameScheduler.Backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class v11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeagueSession<RacingUserIdentity>RacingUserIdentity");

            migrationBuilder.AddColumn<string>(
                name: "RacingUserIdentityId",
                table: "Sessions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SessionRegistration<RacingUserIdentity>",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    SessionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CarId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionRegistration<RacingUserIdentity>", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionRegistration<RacingUserIdentity>_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SessionRegistration<RacingUserIdentity>_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionRegistration<RacingUserIdentity>_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_RacingUserIdentityId",
                table: "Sessions",
                column: "RacingUserIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionRegistration<RacingUserIdentity>_CarId",
                table: "SessionRegistration<RacingUserIdentity>",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionRegistration<RacingUserIdentity>_SessionId",
                table: "SessionRegistration<RacingUserIdentity>",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionRegistration<RacingUserIdentity>_UserId",
                table: "SessionRegistration<RacingUserIdentity>",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_AspNetUsers_RacingUserIdentityId",
                table: "Sessions",
                column: "RacingUserIdentityId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_AspNetUsers_RacingUserIdentityId",
                table: "Sessions");

            migrationBuilder.DropTable(
                name: "SessionRegistration<RacingUserIdentity>");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_RacingUserIdentityId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "RacingUserIdentityId",
                table: "Sessions");

            migrationBuilder.CreateTable(
                name: "LeagueSession<RacingUserIdentity>RacingUserIdentity",
                columns: table => new
                {
                    RegistrationsId = table.Column<string>(type: "TEXT", nullable: false),
                    SessionsId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueSession<RacingUserIdentity>RacingUserIdentity", x => new { x.RegistrationsId, x.SessionsId });
                    table.ForeignKey(
                        name: "FK_LeagueSession<RacingUserIdentity>RacingUserIdentity_AspNetUsers_RegistrationsId",
                        column: x => x.RegistrationsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeagueSession<RacingUserIdentity>RacingUserIdentity_Sessions_SessionsId",
                        column: x => x.SessionsId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeagueSession<RacingUserIdentity>RacingUserIdentity_SessionsId",
                table: "LeagueSession<RacingUserIdentity>RacingUserIdentity",
                column: "SessionsId");
        }
    }
}
