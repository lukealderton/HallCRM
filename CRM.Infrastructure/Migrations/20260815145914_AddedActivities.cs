using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_Activity",
                columns: table => new
                {
                    actId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    actCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    actContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    actJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    actAssignedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    actType = table.Column<int>(type: "int", nullable: false),
                    actSubject = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    actDescription = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    actDueUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    actCompletedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Activity", x => x.actId);
                    table.ForeignKey(
                        name: "FK_T_Activity_T_Company_actCompanyId",
                        column: x => x.actCompanyId,
                        principalTable: "T_Company",
                        principalColumn: "cmpId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_Activity_T_Contacts_actContactId",
                        column: x => x.actContactId,
                        principalTable: "T_Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_Activity_T_Entity_actId",
                        column: x => x.actId,
                        principalTable: "T_Entity",
                        principalColumn: "entId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_Activity_T_Jobs_actJobId",
                        column: x => x.actJobId,
                        principalTable: "T_Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_Activity_actAssignedUserId",
                table: "T_Activity",
                column: "actAssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Activity_actCompanyId",
                table: "T_Activity",
                column: "actCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Activity_actCompletedUtc",
                table: "T_Activity",
                column: "actCompletedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_T_Activity_actCompletedUtc_actDueUtc",
                table: "T_Activity",
                columns: new[] { "actCompletedUtc", "actDueUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_T_Activity_actContactId",
                table: "T_Activity",
                column: "actContactId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Activity_actDueUtc",
                table: "T_Activity",
                column: "actDueUtc");

            migrationBuilder.CreateIndex(
                name: "IX_T_Activity_actJobId",
                table: "T_Activity",
                column: "actJobId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Activity_actType",
                table: "T_Activity",
                column: "actType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_Activity");
        }
    }
}
