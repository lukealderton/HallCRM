using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_Payment",
                columns: table => new
                {
                    payId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    payInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    payAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    payPaymentDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    payMethod = table.Column<int>(type: "int", nullable: false),
                    payReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    payNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Payment", x => x.payId);
                    table.ForeignKey(
                        name: "FK_T_Payment_T_Entity_payId",
                        column: x => x.payId,
                        principalTable: "T_Entity",
                        principalColumn: "entId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_Payment_T_Invoice_payInvoiceId",
                        column: x => x.payInvoiceId,
                        principalTable: "T_Invoice",
                        principalColumn: "invId");
                });

            migrationBuilder.InsertData(
                table: "T_EntityType",
                columns: new[] { "etyId", "etyAlias", "etyCreatedUtc", "etyCustom", "etySystem", "etyName" },
                values: new object[] { 8, "payment", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, true, "Payment" });

            migrationBuilder.CreateIndex(
                name: "IX_T_Payment_payInvoiceId",
                table: "T_Payment",
                column: "payInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Payment_payPaymentDateUtc",
                table: "T_Payment",
                column: "payPaymentDateUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_Payment");

            migrationBuilder.DeleteData(
                table: "T_EntityType",
                keyColumn: "etyId",
                keyValue: 8);
        }
    }
}
