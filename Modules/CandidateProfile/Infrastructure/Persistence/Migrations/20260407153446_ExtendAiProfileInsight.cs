using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrailJobApi.Modules.CandidateProfile.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExtendAiProfileInsight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "AiArchitectureFocus",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "AiCertifications",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "AiCompanyTypes",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "AiContractPreferences",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "AiCoreSkills",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "AiDeliveryPractices",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "AiDomains",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "AiEducationDetails",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "AiExperienceHighlights",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string>(
                name: "AiExperienceLevelYears",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string[]>(
                name: "AiHobbies",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "AiIndustries",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "AiLanguagesSpoken",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "AiLocations",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string>(
                name: "AiManagementScope",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string[]>(
                name: "AiMobilityArea",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "AiMustHaveSkills",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "AiNiceToHaveSkills",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "AiPersonalityTraits",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "AiPreferredJobTitles",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "AiSearchKeywords",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "AiSecondarySkills",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string>(
                name: "AiSeniority",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string[]>(
                name: "AiSoftSkills",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "AiTargetRoles",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "AiWorkModes",
                schema: "candidate_profile",
                table: "CandidateProfiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiArchitectureFocus",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiCertifications",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiCompanyTypes",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiContractPreferences",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiCoreSkills",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiDeliveryPractices",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiDomains",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiEducationDetails",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiExperienceHighlights",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiExperienceLevelYears",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiHobbies",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiIndustries",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiLanguagesSpoken",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiLocations",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiManagementScope",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiMobilityArea",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiMustHaveSkills",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiNiceToHaveSkills",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiPersonalityTraits",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiPreferredJobTitles",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiSearchKeywords",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiSecondarySkills",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiSeniority",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiSoftSkills",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiTargetRoles",
                schema: "candidate_profile",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "AiWorkModes",
                schema: "candidate_profile",
                table: "CandidateProfiles");
        }
    }
}
