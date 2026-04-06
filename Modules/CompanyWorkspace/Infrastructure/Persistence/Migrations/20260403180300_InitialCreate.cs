using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrailJobApi.Modules.CompanyWorkspace.Infrastructure.Persistence.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "company_workspace");

        migrationBuilder.CreateTable(
            name: "JobOpportunities",
            schema: "company_workspace",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
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
                Status = table.Column<int>(type: "integer", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_JobOpportunities", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_JobOpportunities_UserId_Status_UpdatedAtUtc",
            schema: "company_workspace",
            table: "JobOpportunities",
            columns: new[] { "UserId", "Status", "UpdatedAtUtc" });

        migrationBuilder.CreateIndex(
            name: "IX_JobOpportunities_UserId_CompanyName",
            schema: "company_workspace",
            table: "JobOpportunities",
            columns: new[] { "UserId", "CompanyName" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "JobOpportunities", schema: "company_workspace");
    }
}
