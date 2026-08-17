using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceEntityType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "T_EntityType",
                columns: new[] { "etyId", "etyAlias", "etyCreatedUtc", "etyCustom", "etySystem", "etyName" },
                values: new object[] { 7, "invoice", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, true, "Invoice" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "T_EntityType",
                keyColumn: "etyId",
                keyValue: 7);
        }
    }
}
