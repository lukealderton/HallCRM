using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OpToJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_Opportunities");

            migrationBuilder.RenameColumn(
                name: "OpportunityId",
                table: "T_Ticket",
                newName: "JobId");

            migrationBuilder.CreateTable(
                name: "T_Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Stage = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ProbabilityPercent = table.Column<int>(type: "int", nullable: true),
                    ExpectedCloseDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_Jobs_T_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "T_Company",
                        principalColumn: "cmpId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_Jobs_T_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "T_Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_Jobs_T_Entity_Id",
                        column: x => x.Id,
                        principalTable: "T_Entity",
                        principalColumn: "entId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "T_EntityType",
                keyColumn: "etyId",
                keyValue: 3,
                columns: new[] { "etyAlias", "etyName" },
                values: new object[] { "job", "Job" });

            migrationBuilder.CreateIndex(
                name: "IX_T_Jobs_CompanyId",
                table: "T_Jobs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Jobs_ContactId",
                table: "T_Jobs",
                column: "ContactId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_Jobs");

            migrationBuilder.RenameColumn(
                name: "JobId",
                table: "T_Ticket",
                newName: "OpportunityId");

            migrationBuilder.CreateTable(
                name: "T_Opportunities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ExpectedCloseDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ProbabilityPercent = table.Column<int>(type: "int", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Stage = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Opportunities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_Opportunities_T_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "T_Company",
                        principalColumn: "cmpId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_Opportunities_T_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "T_Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_Opportunities_T_Entity_Id",
                        column: x => x.Id,
                        principalTable: "T_Entity",
                        principalColumn: "entId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "T_EntityType",
                keyColumn: "etyId",
                keyValue: 3,
                columns: new[] { "etyAlias", "etyName" },
                values: new object[] { "opportunity", "Opportunity" });

            migrationBuilder.CreateIndex(
                name: "IX_T_Opportunities_CompanyId",
                table: "T_Opportunities",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Opportunities_ContactId",
                table: "T_Opportunities",
                column: "ContactId");
        }
    }
}
