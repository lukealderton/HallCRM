using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameContOppTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_T_Company_CompanyId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_T_Entity_Id",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_Contacts_ContactId",
                table: "Opportunities");

            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_T_Company_CompanyId",
                table: "Opportunities");

            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_T_Entity_Id",
                table: "Opportunities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Opportunities",
                table: "Opportunities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contacts",
                table: "Contacts");

            migrationBuilder.RenameTable(
                name: "Opportunities",
                newName: "T_Opportunities");

            migrationBuilder.RenameTable(
                name: "Contacts",
                newName: "T_Contacts");

            migrationBuilder.RenameIndex(
                name: "IX_Opportunities_ContactId",
                table: "T_Opportunities",
                newName: "IX_T_Opportunities_ContactId");

            migrationBuilder.RenameIndex(
                name: "IX_Opportunities_CompanyId",
                table: "T_Opportunities",
                newName: "IX_T_Opportunities_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Contacts_CompanyId",
                table: "T_Contacts",
                newName: "IX_T_Contacts_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_Opportunities",
                table: "T_Opportunities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_Contacts",
                table: "T_Contacts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_T_Contacts_T_Company_CompanyId",
                table: "T_Contacts",
                column: "CompanyId",
                principalTable: "T_Company",
                principalColumn: "cmpId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Contacts_T_Entity_Id",
                table: "T_Contacts",
                column: "Id",
                principalTable: "T_Entity",
                principalColumn: "entId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Opportunities_T_Company_CompanyId",
                table: "T_Opportunities",
                column: "CompanyId",
                principalTable: "T_Company",
                principalColumn: "cmpId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Opportunities_T_Contacts_ContactId",
                table: "T_Opportunities",
                column: "ContactId",
                principalTable: "T_Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Opportunities_T_Entity_Id",
                table: "T_Opportunities",
                column: "Id",
                principalTable: "T_Entity",
                principalColumn: "entId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_Contacts_T_Company_CompanyId",
                table: "T_Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Contacts_T_Entity_Id",
                table: "T_Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Opportunities_T_Company_CompanyId",
                table: "T_Opportunities");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Opportunities_T_Contacts_ContactId",
                table: "T_Opportunities");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Opportunities_T_Entity_Id",
                table: "T_Opportunities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_Opportunities",
                table: "T_Opportunities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_Contacts",
                table: "T_Contacts");

            migrationBuilder.RenameTable(
                name: "T_Opportunities",
                newName: "Opportunities");

            migrationBuilder.RenameTable(
                name: "T_Contacts",
                newName: "Contacts");

            migrationBuilder.RenameIndex(
                name: "IX_T_Opportunities_ContactId",
                table: "Opportunities",
                newName: "IX_Opportunities_ContactId");

            migrationBuilder.RenameIndex(
                name: "IX_T_Opportunities_CompanyId",
                table: "Opportunities",
                newName: "IX_Opportunities_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_T_Contacts_CompanyId",
                table: "Contacts",
                newName: "IX_Contacts_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Opportunities",
                table: "Opportunities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contacts",
                table: "Contacts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_T_Company_CompanyId",
                table: "Contacts",
                column: "CompanyId",
                principalTable: "T_Company",
                principalColumn: "cmpId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_T_Entity_Id",
                table: "Contacts",
                column: "Id",
                principalTable: "T_Entity",
                principalColumn: "entId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_Contacts_ContactId",
                table: "Opportunities",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_T_Company_CompanyId",
                table: "Opportunities",
                column: "CompanyId",
                principalTable: "T_Company",
                principalColumn: "cmpId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_T_Entity_Id",
                table: "Opportunities",
                column: "Id",
                principalTable: "T_Entity",
                principalColumn: "entId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
