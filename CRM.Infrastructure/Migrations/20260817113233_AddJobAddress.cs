using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJobAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "jobAccessNotes",
                table: "T_Jobs",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "jobAddressLine1",
                table: "T_Jobs",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "jobAddressLine2",
                table: "T_Jobs",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "jobCounty",
                table: "T_Jobs",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "jobPostcode",
                table: "T_Jobs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "jobSiteContactName",
                table: "T_Jobs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "jobSiteContactPhone",
                table: "T_Jobs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "jobTown",
                table: "T_Jobs",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "jobAccessNotes",
                table: "T_Jobs");

            migrationBuilder.DropColumn(
                name: "jobAddressLine1",
                table: "T_Jobs");

            migrationBuilder.DropColumn(
                name: "jobAddressLine2",
                table: "T_Jobs");

            migrationBuilder.DropColumn(
                name: "jobCounty",
                table: "T_Jobs");

            migrationBuilder.DropColumn(
                name: "jobPostcode",
                table: "T_Jobs");

            migrationBuilder.DropColumn(
                name: "jobSiteContactName",
                table: "T_Jobs");

            migrationBuilder.DropColumn(
                name: "jobSiteContactPhone",
                table: "T_Jobs");

            migrationBuilder.DropColumn(
                name: "jobTown",
                table: "T_Jobs");
        }
    }
}
