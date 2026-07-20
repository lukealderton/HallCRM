using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixEntityAuditUserForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_Entity_User_entArchivedByUserId",
                table: "T_Entity");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Entity_User_entCreatedByUserId",
                table: "T_Entity");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Entity_User_entDeletedByUserId",
                table: "T_Entity");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Entity_User_entOwnerUserId",
                table: "T_Entity");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Entity_User_entUpdatedByUserId",
                table: "T_Entity");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.AddForeignKey(
                name: "FK_T_Entity_T_User_entArchivedByUserId",
                table: "T_Entity",
                column: "entArchivedByUserId",
                principalTable: "T_User",
                principalColumn: "usrDomainUserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Entity_T_User_entCreatedByUserId",
                table: "T_Entity",
                column: "entCreatedByUserId",
                principalTable: "T_User",
                principalColumn: "usrDomainUserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Entity_T_User_entDeletedByUserId",
                table: "T_Entity",
                column: "entDeletedByUserId",
                principalTable: "T_User",
                principalColumn: "usrDomainUserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Entity_T_User_entOwnerUserId",
                table: "T_Entity",
                column: "entOwnerUserId",
                principalTable: "T_User",
                principalColumn: "usrDomainUserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Entity_T_User_entUpdatedByUserId",
                table: "T_Entity",
                column: "entUpdatedByUserId",
                principalTable: "T_User",
                principalColumn: "usrDomainUserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_Entity_T_User_entArchivedByUserId",
                table: "T_Entity");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Entity_T_User_entCreatedByUserId",
                table: "T_Entity");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Entity_T_User_entDeletedByUserId",
                table: "T_Entity");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Entity_T_User_entOwnerUserId",
                table: "T_Entity");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Entity_T_User_entUpdatedByUserId",
                table: "T_Entity");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Debug = table.Column<bool>(type: "bit", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailNormalized = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    Forename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InvalidLoginAttempts = table.Column<int>(type: "int", nullable: false),
                    LastEditDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastLoginDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LockedOut = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHashed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PwdLastSetDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RegisterDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeZone = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_T_Entity_User_entArchivedByUserId",
                table: "T_Entity",
                column: "entArchivedByUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Entity_User_entCreatedByUserId",
                table: "T_Entity",
                column: "entCreatedByUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Entity_User_entDeletedByUserId",
                table: "T_Entity",
                column: "entDeletedByUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Entity_User_entOwnerUserId",
                table: "T_Entity",
                column: "entOwnerUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Entity_User_entUpdatedByUserId",
                table: "T_Entity",
                column: "entUpdatedByUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
