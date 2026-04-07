using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrailJobApi.Modules.UserAccess.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteAccessRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteAccessRequests",
                schema: "user_access",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ContactEmail = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    JobOffer = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NotificationStatus = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    NotificationSentAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NotificationLastError = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    NotificationAttemptCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteAccessRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SiteAccessRequests_ContactEmail",
                schema: "user_access",
                table: "SiteAccessRequests",
                column: "ContactEmail");

            migrationBuilder.CreateIndex(
                name: "IX_SiteAccessRequests_CreatedAtUtc",
                schema: "user_access",
                table: "SiteAccessRequests",
                column: "CreatedAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteAccessRequests",
                schema: "user_access");
        }
    }
}
