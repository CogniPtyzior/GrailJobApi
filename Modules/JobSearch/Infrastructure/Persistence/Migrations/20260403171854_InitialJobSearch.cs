using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrailJobApi.Modules.JobSearch.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialJobSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "job_search");

            migrationBuilder.CreateTable(
                name: "SearchSessions",
                schema: "job_search",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastExecutedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchCriteria",
                schema: "job_search",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SearchSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchCriteria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SearchCriteria_SearchSessions_SearchSessionId",
                        column: x => x.SearchSessionId,
                        principalSchema: "job_search",
                        principalTable: "SearchSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SearchResults",
                schema: "job_search",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SearchSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    JobTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OfferUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    OfferDescription = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WorkMode = table.Column<int>(type: "integer", nullable: false),
                    Salary = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TechStack = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    MatchExplanation = table.Column<string>(type: "text", nullable: false),
                    RelevanceScore = table.Column<int>(type: "integer", nullable: false),
                    UserComment = table.Column<string>(type: "text", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SearchResults_SearchSessions_SearchSessionId",
                        column: x => x.SearchSessionId,
                        principalSchema: "job_search",
                        principalTable: "SearchSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SearchCriteria_SearchSessionId_Order",
                schema: "job_search",
                table: "SearchCriteria",
                columns: new[] { "SearchSessionId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_SearchCriteria_SearchSessionId_Text",
                schema: "job_search",
                table: "SearchCriteria",
                columns: new[] { "SearchSessionId", "Text" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SearchResults_SearchSessionId_UpdatedAtUtc",
                schema: "job_search",
                table: "SearchResults",
                columns: new[] { "SearchSessionId", "UpdatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SearchSessions_UserId",
                schema: "job_search",
                table: "SearchSessions",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SearchCriteria",
                schema: "job_search");

            migrationBuilder.DropTable(
                name: "SearchResults",
                schema: "job_search");

            migrationBuilder.DropTable(
                name: "SearchSessions",
                schema: "job_search");
        }
    }
}
