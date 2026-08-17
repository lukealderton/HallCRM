using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJobService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_Service",
                columns: table => new
                {
                    svcId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    svcName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    svcDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    svcDefaultPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    svcNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Service", x => x.svcId);
                    table.ForeignKey(
                        name: "FK_T_Service_T_Entity_svcId",
                        column: x => x.svcId,
                        principalTable: "T_Entity",
                        principalColumn: "entId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_JobService",
                columns: table => new
                {
                    jbsJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    jbsServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_JobService", x => new { x.jbsJobId, x.jbsServiceId });
                    table.ForeignKey(
                        name: "FK_T_JobService_T_Jobs_jbsJobId",
                        column: x => x.jbsJobId,
                        principalTable: "T_Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_JobService_T_Service_jbsServiceId",
                        column: x => x.jbsServiceId,
                        principalTable: "T_Service",
                        principalColumn: "svcId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "T_EntityType",
                columns: new[] { "etyId", "etyAlias", "etyCreatedUtc", "etyCustom", "etySystem", "etyName" },
                values: new object[] { 6, "service", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, true, "Service" });

            migrationBuilder.CreateIndex(
                name: "IX_T_JobService_jbsServiceId",
                table: "T_JobService",
                column: "jbsServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_JobService");

            migrationBuilder.DropTable(
                name: "T_Service");

            migrationBuilder.DeleteData(
                table: "T_EntityType",
                keyColumn: "etyId",
                keyValue: 6);
        }
    }
}
