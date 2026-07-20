using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_Company",
                columns: table => new
                {
                    cmpId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    cmpName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    cmpTradingName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    cmpWebsite = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    cmpPrimaryEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    cmpPrimaryPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    cmpAdrLine1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    cmpAdrLine2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    cmpAdrTown = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    cmpAdrCounty = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    cmpAdrPostcode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    cmpAdrCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    cmpCompanyNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    cmpVatNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    cmpIndustry = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    cmpNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Company", x => x.cmpId);
                    table.ForeignKey(
                        name: "FK_T_Company_T_Entity_cmpId",
                        column: x => x.cmpId,
                        principalTable: "T_Entity",
                        principalColumn: "entId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_Company_cmpCompanyNumber",
                table: "T_Company",
                column: "cmpCompanyNumber");

            migrationBuilder.CreateIndex(
                name: "IX_T_Company_cmpName",
                table: "T_Company",
                column: "cmpName");

            migrationBuilder.CreateIndex(
                name: "IX_T_Company_cmpPrimaryEmail",
                table: "T_Company",
                column: "cmpPrimaryEmail");

            migrationBuilder.CreateIndex(
                name: "IX_T_Company_cmpPrimaryPhone",
                table: "T_Company",
                column: "cmpPrimaryPhone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_Company");
        }
    }
}
