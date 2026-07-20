using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCrmEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_EntityType",
                columns: table => new
                {
                    etyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    etyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    etyAlias = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    etySystem = table.Column<bool>(type: "bit", nullable: false),
                    etyCustom = table.Column<bool>(type: "bit", nullable: false),
                    etyCreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_EntityType", x => x.etyId);
                });

            migrationBuilder.CreateTable(
                name: "T_Role",
                columns: table => new
                {
                    rolId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    rolName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    rolNameNorm = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    rolConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Role", x => x.rolId);
                });

            migrationBuilder.CreateTable(
                name: "T_User",
                columns: table => new
                {
                    usrId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    usrDomainUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastLoginUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    usrUsername = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    usrUsernameNorm = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    usrEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    usrEmailNorm = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    usrEmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    usrPasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    usrSecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    usrConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    usrPhone = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    usrPhoneConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    usrTwoFactor = table.Column<bool>(type: "bit", nullable: false),
                    usrLockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    usrLockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    usrAccessFailed = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_User", x => x.usrId);
                    table.UniqueConstraint("AK_T_User_usrDomainUserId", x => x.usrDomainUserId);
                });

            migrationBuilder.CreateTable(
                name: "T_RoleClaim",
                columns: table => new
                {
                    rclId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rclRoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    rclClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    rclClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_RoleClaim", x => x.rclId);
                    table.ForeignKey(
                        name: "FK_T_RoleClaim_T_Role_rclRoleId",
                        column: x => x.rclRoleId,
                        principalTable: "T_Role",
                        principalColumn: "rolId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserPasskeys",
                columns: table => new
                {
                    CredentialId = table.Column<byte[]>(type: "varbinary(1024)", maxLength: 1024, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserPasskeys", x => x.CredentialId);
                    table.ForeignKey(
                        name: "FK_AspNetUserPasskeys_T_User_UserId",
                        column: x => x.UserId,
                        principalTable: "T_User",
                        principalColumn: "usrId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_UserClaim",
                columns: table => new
                {
                    ucmId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ucmUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ucmClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ucmClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_UserClaim", x => x.ucmId);
                    table.ForeignKey(
                        name: "FK_T_UserClaim_T_User_ucmUserId",
                        column: x => x.ucmUserId,
                        principalTable: "T_User",
                        principalColumn: "usrId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_UserLogin",
                columns: table => new
                {
                    lgnProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    lgnProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    lgnProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lgnUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_UserLogin", x => new { x.lgnProvider, x.lgnProviderKey });
                    table.ForeignKey(
                        name: "FK_T_UserLogin_T_User_lgnUserId",
                        column: x => x.lgnUserId,
                        principalTable: "T_User",
                        principalColumn: "usrId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_UserProfile",
                columns: table => new
                {
                    uspId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    uspForename = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    uspSurname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    uspMobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    uspAdrLine1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    uspAdrLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    uspAdrTown = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    uspAdrCounty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    uspAdrPostcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    uspAdrCountry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    uspStripeCustomerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    uspCreatedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    uspUpdatedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_UserProfile", x => x.uspId);
                    table.ForeignKey(
                        name: "FK_T_UserProfile_T_User_uspId",
                        column: x => x.uspId,
                        principalTable: "T_User",
                        principalColumn: "usrDomainUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_UserRole",
                columns: table => new
                {
                    urlUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    urlRoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_UserRole", x => new { x.urlUserId, x.urlRoleId });
                    table.ForeignKey(
                        name: "FK_T_UserRole_T_Role_urlRoleId",
                        column: x => x.urlRoleId,
                        principalTable: "T_Role",
                        principalColumn: "rolId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_UserRole_T_User_urlUserId",
                        column: x => x.urlUserId,
                        principalTable: "T_User",
                        principalColumn: "usrId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_UserToken",
                columns: table => new
                {
                    utnUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    utnLoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    utnName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    utnValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_UserToken", x => new { x.utnUserId, x.utnLoginProvider, x.utnName });
                    table.ForeignKey(
                        name: "FK_T_UserToken_T_User_utnUserId",
                        column: x => x.utnUserId,
                        principalTable: "T_User",
                        principalColumn: "usrId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_Entity",
                columns: table => new
                {
                    entId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    entTypeId = table.Column<int>(type: "int", nullable: false),
                    entDisplayName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    entOwnerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    entCreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    entCreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    entUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    entUpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    entArchivedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    entArchivedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    entDeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    entDeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    entRowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Entity", x => x.entId);
                    table.ForeignKey(
                        name: "FK_T_Entity_T_EntityType_entTypeId",
                        column: x => x.entTypeId,
                        principalTable: "T_EntityType",
                        principalColumn: "etyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_Entity_T_UserProfile_entArchivedByUserId",
                        column: x => x.entArchivedByUserId,
                        principalTable: "T_UserProfile",
                        principalColumn: "uspId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_Entity_T_UserProfile_entCreatedByUserId",
                        column: x => x.entCreatedByUserId,
                        principalTable: "T_UserProfile",
                        principalColumn: "uspId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_Entity_T_UserProfile_entDeletedByUserId",
                        column: x => x.entDeletedByUserId,
                        principalTable: "T_UserProfile",
                        principalColumn: "uspId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_Entity_T_UserProfile_entOwnerUserId",
                        column: x => x.entOwnerUserId,
                        principalTable: "T_UserProfile",
                        principalColumn: "uspId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_Entity_T_UserProfile_entUpdatedByUserId",
                        column: x => x.entUpdatedByUserId,
                        principalTable: "T_UserProfile",
                        principalColumn: "uspId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "T_EntityType",
                columns: new[] { "etyId", "etyAlias", "etyCreatedUtc", "etyCustom", "etySystem", "etyName" },
                values: new object[,]
                {
                    { 1, "company", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, true, "Company" },
                    { 2, "contact", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, true, "Contact" },
                    { 3, "opportunity", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, true, "Opportunity" },
                    { 4, "job", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, true, "Job" },
                    { 5, "activity", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, true, "Activity" },
                    { 100, "custom-record", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Custom Record" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserPasskeys_UserId",
                table: "AspNetUserPasskeys",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Entity_entArchivedByUserId",
                table: "T_Entity",
                column: "entArchivedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Entity_entArchivedUtc",
                table: "T_Entity",
                column: "entArchivedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_T_Entity_entCreatedByUserId",
                table: "T_Entity",
                column: "entCreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Entity_entCreatedUtc",
                table: "T_Entity",
                column: "entCreatedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_T_Entity_entDeletedByUserId",
                table: "T_Entity",
                column: "entDeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Entity_entDeletedUtc",
                table: "T_Entity",
                column: "entDeletedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_T_Entity_entDisplayName",
                table: "T_Entity",
                column: "entDisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_T_Entity_entOwnerUserId",
                table: "T_Entity",
                column: "entOwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Entity_entTypeId",
                table: "T_Entity",
                column: "entTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Entity_entTypeId_entDeletedUtc",
                table: "T_Entity",
                columns: new[] { "entTypeId", "entDeletedUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_T_Entity_entUpdatedByUserId",
                table: "T_Entity",
                column: "entUpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_T_EntityType_etyAlias",
                table: "T_EntityType",
                column: "etyAlias",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "T_Role",
                column: "rolNameNorm",
                unique: true,
                filter: "[rolNameNorm] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_T_RoleClaim_rclRoleId",
                table: "T_RoleClaim",
                column: "rclRoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "T_User",
                column: "usrEmailNorm");

            migrationBuilder.CreateIndex(
                name: "IX_T_User_usrDomainUserId",
                table: "T_User",
                column: "usrDomainUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "T_User",
                column: "usrUsernameNorm",
                unique: true,
                filter: "[usrUsernameNorm] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_T_UserClaim_ucmUserId",
                table: "T_UserClaim",
                column: "ucmUserId");

            migrationBuilder.CreateIndex(
                name: "IX_T_UserLogin_lgnUserId",
                table: "T_UserLogin",
                column: "lgnUserId");

            migrationBuilder.CreateIndex(
                name: "IX_T_UserRole_urlRoleId",
                table: "T_UserRole",
                column: "urlRoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetUserPasskeys");

            migrationBuilder.DropTable(
                name: "T_Entity");

            migrationBuilder.DropTable(
                name: "T_RoleClaim");

            migrationBuilder.DropTable(
                name: "T_UserClaim");

            migrationBuilder.DropTable(
                name: "T_UserLogin");

            migrationBuilder.DropTable(
                name: "T_UserRole");

            migrationBuilder.DropTable(
                name: "T_UserToken");

            migrationBuilder.DropTable(
                name: "T_EntityType");

            migrationBuilder.DropTable(
                name: "T_UserProfile");

            migrationBuilder.DropTable(
                name: "T_Role");

            migrationBuilder.DropTable(
                name: "T_User");
        }
    }
}
