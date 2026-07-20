using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_Ticket",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OpportunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssignedToUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsChargeable = table.Column<bool>(type: "bit", nullable: false),
                    IsInvoiced = table.Column<bool>(type: "bit", nullable: false),
                    EstimatedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    QuotedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    InvoiceValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EstimatedMinutes = table.Column<int>(type: "int", nullable: true),
                    ActualMinutes = table.Column<int>(type: "int", nullable: false),
                    DueUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InternalNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Ticket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_Ticket_T_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "T_Company",
                        principalColumn: "cmpId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_Ticket_T_Entity_Id",
                        column: x => x.Id,
                        principalTable: "T_Entity",
                        principalColumn: "entId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_TicketComment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsInternal = table.Column<bool>(type: "bit", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_TicketComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_TicketComment_T_Ticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "T_Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_TicketStatusHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreviousStatus = table.Column<int>(type: "int", nullable: true),
                    NewStatus = table.Column<int>(type: "int", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChangedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_TicketStatusHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_TicketStatusHistory_T_Ticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "T_Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_TicketTimeEntry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Minutes = table.Column<int>(type: "int", nullable: false),
                    IsChargeable = table.Column<bool>(type: "bit", nullable: false),
                    IsInvoiced = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_TicketTimeEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_TicketTimeEntry_T_Ticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "T_Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "T_EntityType",
                keyColumn: "etyId",
                keyValue: 4,
                columns: new[] { "etyAlias", "etyName" },
                values: new object[] { "ticket", "Ticket" });

            migrationBuilder.CreateIndex(
                name: "IX_T_Ticket_AssignedToUserId",
                table: "T_Ticket",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Ticket_CompanyId",
                table: "T_Ticket",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Ticket_ContactId",
                table: "T_Ticket",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Ticket_DueUtc",
                table: "T_Ticket",
                column: "DueUtc");

            migrationBuilder.CreateIndex(
                name: "IX_T_Ticket_IsChargeable",
                table: "T_Ticket",
                column: "IsChargeable");

            migrationBuilder.CreateIndex(
                name: "IX_T_Ticket_IsInvoiced",
                table: "T_Ticket",
                column: "IsInvoiced");

            migrationBuilder.CreateIndex(
                name: "IX_T_Ticket_Priority",
                table: "T_Ticket",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_T_Ticket_Status",
                table: "T_Ticket",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_T_Ticket_Type",
                table: "T_Ticket",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_T_TicketComment_CreatedUtc",
                table: "T_TicketComment",
                column: "CreatedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_T_TicketComment_TicketId",
                table: "T_TicketComment",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_T_TicketStatusHistory_ChangedUtc",
                table: "T_TicketStatusHistory",
                column: "ChangedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_T_TicketStatusHistory_TicketId",
                table: "T_TicketStatusHistory",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_T_TicketTimeEntry_CreatedUtc",
                table: "T_TicketTimeEntry",
                column: "CreatedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_T_TicketTimeEntry_IsInvoiced",
                table: "T_TicketTimeEntry",
                column: "IsInvoiced");

            migrationBuilder.CreateIndex(
                name: "IX_T_TicketTimeEntry_TicketId",
                table: "T_TicketTimeEntry",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_T_TicketTimeEntry_UserId",
                table: "T_TicketTimeEntry",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_TicketComment");

            migrationBuilder.DropTable(
                name: "T_TicketStatusHistory");

            migrationBuilder.DropTable(
                name: "T_TicketTimeEntry");

            migrationBuilder.DropTable(
                name: "T_Ticket");

            migrationBuilder.UpdateData(
                table: "T_EntityType",
                keyColumn: "etyId",
                keyValue: 4,
                columns: new[] { "etyAlias", "etyName" },
                values: new object[] { "job", "Job" });
        }
    }
}
