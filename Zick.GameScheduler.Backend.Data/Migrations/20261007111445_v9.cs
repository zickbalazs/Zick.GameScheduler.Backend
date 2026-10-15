using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zick.GameScheduler.Backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class v9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SessionName = table.Column<string>(type: "TEXT", nullable: false),
                    LeagueSessionRacingUserIdentityId = table.Column<Guid>(name: "LeagueSession<RacingUserIdentity>Id", type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Results_Sessions_LeagueSession<RacingUserIdentity>Id",
                        column: x => x.LeagueSessionRacingUserIdentityId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ResultDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDisqualified = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPenalized = table.Column<bool>(type: "INTEGER", nullable: false),
                    PenaltyTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    BestLapTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    TotalTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    CarName = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    SessionResultId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultDetails_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ResultDetails_Results_SessionResultId",
                        column: x => x.SessionResultId,
                        principalTable: "Results",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResultDetails_SessionResultId",
                table: "ResultDetails",
                column: "SessionResultId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultDetails_UserId",
                table: "ResultDetails",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Results_LeagueSession<RacingUserIdentity>Id",
                table: "Results",
                column: "LeagueSession<RacingUserIdentity>Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResultDetails");

            migrationBuilder.DropTable(
                name: "Results");
        }
    }
}
