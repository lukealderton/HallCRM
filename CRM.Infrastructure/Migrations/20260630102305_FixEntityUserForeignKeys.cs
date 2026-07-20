using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixEntityUserForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_Entity_T_UserProfile_entArchivedByUserId",
                table: "T_Entity");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Entity_T_UserProfile_entCreatedByUserId",
                table: "T_Entity");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Entity_T_UserProfile_entDeletedByUserId",
                table: "T_Entity");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Entity_T_UserProfile_entOwnerUserId",
                table: "T_Entity");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Entity_T_UserProfile_entUpdatedByUserId",
                table: "T_Entity");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmailNormalized = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHashed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PwdLastSetDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Debug = table.Column<bool>(type: "bit", nullable: false),
                    InvalidLoginAttempts = table.Column<int>(type: "int", nullable: false),
                    LastEditDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LockedOut = table.Column<bool>(type: "bit", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Forename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    RegisterDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastLoginDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ProfileImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "FK_T_Entity_T_UserProfile_entArchivedByUserId",
                table: "T_Entity",
                column: "entArchivedByUserId",
                principalTable: "T_UserProfile",
                principalColumn: "uspId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Entity_T_UserProfile_entCreatedByUserId",
                table: "T_Entity",
                column: "entCreatedByUserId",
                principalTable: "T_UserProfile",
                principalColumn: "uspId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Entity_T_UserProfile_entDeletedByUserId",
                table: "T_Entity",
                column: "entDeletedByUserId",
                principalTable: "T_UserProfile",
                principalColumn: "uspId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Entity_T_UserProfile_entOwnerUserId",
                table: "T_Entity",
                column: "entOwnerUserId",
                principalTable: "T_UserProfile",
                principalColumn: "uspId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Entity_T_UserProfile_entUpdatedByUserId",
                table: "T_Entity",
                column: "entUpdatedByUserId",
                principalTable: "T_UserProfile",
                principalColumn: "uspId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
