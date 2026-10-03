using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContractorsAndJobAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "jobAssignedUserId",
                table: "T_Jobs",
                newName: "jobAssignedContractorId");

            migrationBuilder.CreateTable(
                name: "T_Contractor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Contractor", x => x.Id);
                });

            // Preserve each existing assignment as an independent contractor record.
            // IDs remain unchanged so the renamed job column keeps its current values.
            migrationBuilder.Sql("""
                INSERT INTO [T_Contractor] ([Id], [Name], [Email], [Phone], [Notes], [Enabled], [CreatedUtc])
                SELECT assigned.[ContractorId],
                    LEFT(COALESCE(
                        NULLIF(LTRIM(RTRIM(CONCAT(staff.[Forename], ' ', staff.[Surname]))), ''),
                        NULLIF(staff.[usrEmail], ''),
                        NULLIF(staff.[usrUsername], ''),
                        'Imported contractor'), 200),
                    LEFT(NULLIF(staff.[usrEmail], ''), 254),
                    LEFT(NULLIF(staff.[usrPhone], ''), 50),
                    'Imported from an existing job assignment.',
                    COALESCE(staff.[usrEnabled], CAST(1 AS bit)),
                    SYSUTCDATETIME()
                FROM (
                    SELECT DISTINCT [jobAssignedContractorId] AS [ContractorId]
                    FROM [T_Jobs]
                    WHERE [jobAssignedContractorId] IS NOT NULL
                ) assigned
                LEFT JOIN [T_User] staff ON staff.[usrDomainUserId] = assigned.[ContractorId];
                """);

            migrationBuilder.CreateIndex(
                name: "IX_T_Jobs_jobAssignedContractorId",
                table: "T_Jobs",
                column: "jobAssignedContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Contractor_Enabled_Name",
                table: "T_Contractor",
                columns: new[] { "Enabled", "Name" });

            migrationBuilder.AddForeignKey(
                name: "FK_T_Jobs_T_Contractor_jobAssignedContractorId",
                table: "T_Jobs",
                column: "jobAssignedContractorId",
                principalTable: "T_Contractor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_Jobs_T_Contractor_jobAssignedContractorId",
                table: "T_Jobs");

            migrationBuilder.DropTable(
                name: "T_Contractor");

            migrationBuilder.DropIndex(
                name: "IX_T_Jobs_jobAssignedContractorId",
                table: "T_Jobs");

            migrationBuilder.RenameColumn(
                name: "jobAssignedContractorId",
                table: "T_Jobs",
                newName: "jobAssignedUserId");
        }
    }
}
