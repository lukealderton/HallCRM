using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_Log",
                columns: table => new
                {
                    logId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    logTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    logType = table.Column<int>(type: "int", nullable: false),
                    logArea = table.Column<int>(type: "int", nullable: false),
                    logMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    logRelId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    logRelType = table.Column<int>(type: "int", nullable: false),
                    logText = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Log", x => x.logId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_Log_logArea_logTimestamp",
                table: "T_Log",
                columns: new[] { "logArea", "logTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_T_Log_logMemberId_logTimestamp",
                table: "T_Log",
                columns: new[] { "logMemberId", "logTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_T_Log_logRelType_logRelId_logTimestamp",
                table: "T_Log",
                columns: new[] { "logRelType", "logRelId", "logTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_T_Log_logTimestamp",
                table: "T_Log",
                column: "logTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_T_Log_logType_logTimestamp",
                table: "T_Log",
                columns: new[] { "logType", "logTimestamp" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_Log");
        }
    }
}
