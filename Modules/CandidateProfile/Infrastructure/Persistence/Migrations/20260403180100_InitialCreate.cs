using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrailJobApi.Modules.CandidateProfile.Infrastructure.Persistence.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "candidate_profile");

        migrationBuilder.CreateTable(
            name: "CandidateProfiles",
            schema: "candidate_profile",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                OriginalFileName = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                SizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                ExtractedText = table.Column<string>(type: "text", nullable: false),
                SourceType = table.Column<int>(type: "integer", nullable: false),
                AiTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                AiSummary = table.Column<string>(type: "text", nullable: false),
                ImportedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CandidateProfiles", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CandidateProfiles_UserId",
            schema: "candidate_profile",
            table: "CandidateProfiles",
            column: "UserId",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CandidateProfiles", schema: "candidate_profile");
    }
}
