using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrailJobApi.Modules.UserAccess.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetLinkTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PasswordResetLinkLastSendSucceeded",
                schema: "user_access",
                table: "Users",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetLinkSentAtUtc",
                schema: "user_access",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordResetLinkLastSendSucceeded",
                schema: "user_access",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordResetLinkSentAtUtc",
                schema: "user_access",
                table: "Users");
        }
    }
}
