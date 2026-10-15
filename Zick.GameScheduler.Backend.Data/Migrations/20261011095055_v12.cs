using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zick.GameScheduler.Backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class v12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionRegistration<RacingUserIdentity>_AspNetUsers_UserId",
                table: "SessionRegistration<RacingUserIdentity>");

            migrationBuilder.DropForeignKey(
                name: "FK_SessionRegistration<RacingUserIdentity>_Cars_CarId",
                table: "SessionRegistration<RacingUserIdentity>");

            migrationBuilder.DropForeignKey(
                name: "FK_SessionRegistration<RacingUserIdentity>_Sessions_SessionId",
                table: "SessionRegistration<RacingUserIdentity>");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SessionRegistration<RacingUserIdentity>",
                table: "SessionRegistration<RacingUserIdentity>");

            migrationBuilder.RenameTable(
                name: "SessionRegistration<RacingUserIdentity>",
                newName: "Registrations");

            migrationBuilder.RenameIndex(
                name: "IX_SessionRegistration<RacingUserIdentity>_UserId",
                table: "Registrations",
                newName: "IX_Registrations_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_SessionRegistration<RacingUserIdentity>_SessionId",
                table: "Registrations",
                newName: "IX_Registrations_SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_SessionRegistration<RacingUserIdentity>_CarId",
                table: "Registrations",
                newName: "IX_Registrations_CarId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Registrations",
                table: "Registrations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_AspNetUsers_UserId",
                table: "Registrations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Cars_CarId",
                table: "Registrations",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Sessions_SessionId",
                table: "Registrations",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_AspNetUsers_UserId",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Cars_CarId",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Sessions_SessionId",
                table: "Registrations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Registrations",
                table: "Registrations");

            migrationBuilder.RenameTable(
                name: "Registrations",
                newName: "SessionRegistration<RacingUserIdentity>");

            migrationBuilder.RenameIndex(
                name: "IX_Registrations_UserId",
                table: "SessionRegistration<RacingUserIdentity>",
                newName: "IX_SessionRegistration<RacingUserIdentity>_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Registrations_SessionId",
                table: "SessionRegistration<RacingUserIdentity>",
                newName: "IX_SessionRegistration<RacingUserIdentity>_SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_Registrations_CarId",
                table: "SessionRegistration<RacingUserIdentity>",
                newName: "IX_SessionRegistration<RacingUserIdentity>_CarId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SessionRegistration<RacingUserIdentity>",
                table: "SessionRegistration<RacingUserIdentity>",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionRegistration<RacingUserIdentity>_AspNetUsers_UserId",
                table: "SessionRegistration<RacingUserIdentity>",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SessionRegistration<RacingUserIdentity>_Cars_CarId",
                table: "SessionRegistration<RacingUserIdentity>",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SessionRegistration<RacingUserIdentity>_Sessions_SessionId",
                table: "SessionRegistration<RacingUserIdentity>",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
