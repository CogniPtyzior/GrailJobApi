using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrailJobApi.Modules.CompanyWorkspace.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeJobOpportunityUserCompanyUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobOpportunities_UserId_CompanyName",
                schema: "company_workspace",
                table: "JobOpportunities");

            migrationBuilder.CreateIndex(
                name: "IX_JobOpportunities_UserId_CompanyName",
                schema: "company_workspace",
                table: "JobOpportunities",
                columns: new[] { "UserId", "CompanyName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobOpportunities_UserId_CompanyName",
                schema: "company_workspace",
                table: "JobOpportunities");

            migrationBuilder.CreateIndex(
                name: "IX_JobOpportunities_UserId_CompanyName",
                schema: "company_workspace",
                table: "JobOpportunities",
                columns: new[] { "UserId", "CompanyName" });
        }
    }
}
