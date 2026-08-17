using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_Invoice",
                columns: table => new
                {
                    invId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    invJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    invCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    invContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    invNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    invStatus = table.Column<int>(type: "int", nullable: false),
                    invIssueDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    invDueDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    invCustomerName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    invAddressLine1 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    invAddressLine2 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    invTown = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    invCounty = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    invPostcode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    invNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Invoice", x => x.invId);
                    table.ForeignKey(
                        name: "FK_T_Invoice_T_Company_invCompanyId",
                        column: x => x.invCompanyId,
                        principalTable: "T_Company",
                        principalColumn: "cmpId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_Invoice_T_Contacts_invContactId",
                        column: x => x.invContactId,
                        principalTable: "T_Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_Invoice_T_Entity_invId",
                        column: x => x.invId,
                        principalTable: "T_Entity",
                        principalColumn: "entId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_Invoice_T_Jobs_invJobId",
                        column: x => x.invJobId,
                        principalTable: "T_Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "T_InvoiceLine",
                columns: table => new
                {
                    inlId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    inlInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    inlServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    inlSortOrder = table.Column<int>(type: "int", nullable: false),
                    inlDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    inlQuantity = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    inlUnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_InvoiceLine", x => x.inlId);
                    table.ForeignKey(
                        name: "FK_T_InvoiceLine_T_Invoice_inlInvoiceId",
                        column: x => x.inlInvoiceId,
                        principalTable: "T_Invoice",
                        principalColumn: "invId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_InvoiceLine_T_Service_inlServiceId",
                        column: x => x.inlServiceId,
                        principalTable: "T_Service",
                        principalColumn: "svcId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_Invoice_invCompanyId",
                table: "T_Invoice",
                column: "invCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Invoice_invContactId",
                table: "T_Invoice",
                column: "invContactId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Invoice_invJobId",
                table: "T_Invoice",
                column: "invJobId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Invoice_invNumber",
                table: "T_Invoice",
                column: "invNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_Invoice_invStatus",
                table: "T_Invoice",
                column: "invStatus");

            migrationBuilder.CreateIndex(
                name: "IX_T_InvoiceLine_inlInvoiceId",
                table: "T_InvoiceLine",
                column: "inlInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_T_InvoiceLine_inlServiceId",
                table: "T_InvoiceLine",
                column: "inlServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_InvoiceLine");

            migrationBuilder.DropTable(
                name: "T_Invoice");
        }
    }
}
