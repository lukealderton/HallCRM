using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserAbstraction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_UserProfile_T_User_uspId",
                table: "T_UserProfile");

            migrationBuilder.RenameColumn(
                name: "LastLoginUtc",
                table: "T_User",
                newName: "usrLastLoginUtc");

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "T_User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Forename",
                table: "T_User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "T_User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "usrCreatedUtc",
                table: "T_User",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "usrUpdatedUtc",
                table: "T_User",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "T_User");

            migrationBuilder.DropColumn(
                name: "Forename",
                table: "T_User");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "T_User");

            migrationBuilder.DropColumn(
                name: "usrCreatedUtc",
                table: "T_User");

            migrationBuilder.DropColumn(
                name: "usrUpdatedUtc",
                table: "T_User");

            migrationBuilder.RenameColumn(
                name: "usrLastLoginUtc",
                table: "T_User",
                newName: "LastLoginUtc");

            migrationBuilder.AddForeignKey(
                name: "FK_T_UserProfile_T_User_uspId",
                table: "T_UserProfile",
                column: "uspId",
                principalTable: "T_User",
                principalColumn: "usrDomainUserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
