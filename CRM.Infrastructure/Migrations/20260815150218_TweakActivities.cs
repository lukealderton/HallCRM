using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TweakActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_Activity_T_Company_actCompanyId",
                table: "T_Activity");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Activity_T_Contacts_actContactId",
                table: "T_Activity");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Activity_T_Entity_actId",
                table: "T_Activity");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Activity_T_Jobs_actJobId",
                table: "T_Activity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_Activity",
                table: "T_Activity");

            migrationBuilder.RenameTable(
                name: "T_Activity",
                newName: "T_Activities");

            migrationBuilder.RenameColumn(
                name: "actType",
                table: "T_Activities",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "actSubject",
                table: "T_Activities",
                newName: "Subject");

            migrationBuilder.RenameColumn(
                name: "actJobId",
                table: "T_Activities",
                newName: "JobId");

            migrationBuilder.RenameColumn(
                name: "actDueUtc",
                table: "T_Activities",
                newName: "DueUtc");

            migrationBuilder.RenameColumn(
                name: "actDescription",
                table: "T_Activities",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "actContactId",
                table: "T_Activities",
                newName: "ContactId");

            migrationBuilder.RenameColumn(
                name: "actCompletedUtc",
                table: "T_Activities",
                newName: "CompletedUtc");

            migrationBuilder.RenameColumn(
                name: "actCompanyId",
                table: "T_Activities",
                newName: "CompanyId");

            migrationBuilder.RenameColumn(
                name: "actAssignedUserId",
                table: "T_Activities",
                newName: "AssignedUserId");

            migrationBuilder.RenameColumn(
                name: "actId",
                table: "T_Activities",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_T_Activity_actType",
                table: "T_Activities",
                newName: "IX_T_Activities_Type");

            migrationBuilder.RenameIndex(
                name: "IX_T_Activity_actJobId",
                table: "T_Activities",
                newName: "IX_T_Activities_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_T_Activity_actDueUtc",
                table: "T_Activities",
                newName: "IX_T_Activities_DueUtc");

            migrationBuilder.RenameIndex(
                name: "IX_T_Activity_actContactId",
                table: "T_Activities",
                newName: "IX_T_Activities_ContactId");

            migrationBuilder.RenameIndex(
                name: "IX_T_Activity_actCompletedUtc_actDueUtc",
                table: "T_Activities",
                newName: "IX_T_Activities_CompletedUtc_DueUtc");

            migrationBuilder.RenameIndex(
                name: "IX_T_Activity_actCompletedUtc",
                table: "T_Activities",
                newName: "IX_T_Activities_CompletedUtc");

            migrationBuilder.RenameIndex(
                name: "IX_T_Activity_actCompanyId",
                table: "T_Activities",
                newName: "IX_T_Activities_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_T_Activity_actAssignedUserId",
                table: "T_Activities",
                newName: "IX_T_Activities_AssignedUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_Activities",
                table: "T_Activities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_T_Activities_T_Company_CompanyId",
                table: "T_Activities",
                column: "CompanyId",
                principalTable: "T_Company",
                principalColumn: "cmpId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Activities_T_Contacts_ContactId",
                table: "T_Activities",
                column: "ContactId",
                principalTable: "T_Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Activities_T_Entity_Id",
                table: "T_Activities",
                column: "Id",
                principalTable: "T_Entity",
                principalColumn: "entId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Activities_T_Jobs_JobId",
                table: "T_Activities",
                column: "JobId",
                principalTable: "T_Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Activities_T_User_AssignedUserId",
                table: "T_Activities",
                column: "AssignedUserId",
                principalTable: "T_User",
                principalColumn: "usrDomainUserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_Activities_T_Company_CompanyId",
                table: "T_Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Activities_T_Contacts_ContactId",
                table: "T_Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Activities_T_Entity_Id",
                table: "T_Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Activities_T_Jobs_JobId",
                table: "T_Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Activities_T_User_AssignedUserId",
                table: "T_Activities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_Activities",
                table: "T_Activities");

            migrationBuilder.RenameTable(
                name: "T_Activities",
                newName: "T_Activity");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "T_Activity",
                newName: "actType");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "T_Activity",
                newName: "actSubject");

            migrationBuilder.RenameColumn(
                name: "JobId",
                table: "T_Activity",
                newName: "actJobId");

            migrationBuilder.RenameColumn(
                name: "DueUtc",
                table: "T_Activity",
                newName: "actDueUtc");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "T_Activity",
                newName: "actDescription");

            migrationBuilder.RenameColumn(
                name: "ContactId",
                table: "T_Activity",
                newName: "actContactId");

            migrationBuilder.RenameColumn(
                name: "CompletedUtc",
                table: "T_Activity",
                newName: "actCompletedUtc");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "T_Activity",
                newName: "actCompanyId");

            migrationBuilder.RenameColumn(
                name: "AssignedUserId",
                table: "T_Activity",
                newName: "actAssignedUserId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "T_Activity",
                newName: "actId");

            migrationBuilder.RenameIndex(
                name: "IX_T_Activities_Type",
                table: "T_Activity",
                newName: "IX_T_Activity_actType");

            migrationBuilder.RenameIndex(
                name: "IX_T_Activities_JobId",
                table: "T_Activity",
                newName: "IX_T_Activity_actJobId");

            migrationBuilder.RenameIndex(
                name: "IX_T_Activities_DueUtc",
                table: "T_Activity",
                newName: "IX_T_Activity_actDueUtc");

            migrationBuilder.RenameIndex(
                name: "IX_T_Activities_ContactId",
                table: "T_Activity",
                newName: "IX_T_Activity_actContactId");

            migrationBuilder.RenameIndex(
                name: "IX_T_Activities_CompletedUtc_DueUtc",
                table: "T_Activity",
                newName: "IX_T_Activity_actCompletedUtc_actDueUtc");

            migrationBuilder.RenameIndex(
                name: "IX_T_Activities_CompletedUtc",
                table: "T_Activity",
                newName: "IX_T_Activity_actCompletedUtc");

            migrationBuilder.RenameIndex(
                name: "IX_T_Activities_CompanyId",
                table: "T_Activity",
                newName: "IX_T_Activity_actCompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_T_Activities_AssignedUserId",
                table: "T_Activity",
                newName: "IX_T_Activity_actAssignedUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_Activity",
                table: "T_Activity",
                column: "actId");

            migrationBuilder.AddForeignKey(
                name: "FK_T_Activity_T_Company_actCompanyId",
                table: "T_Activity",
                column: "actCompanyId",
                principalTable: "T_Company",
                principalColumn: "cmpId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Activity_T_Contacts_actContactId",
                table: "T_Activity",
                column: "actContactId",
                principalTable: "T_Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Activity_T_Entity_actId",
                table: "T_Activity",
                column: "actId",
                principalTable: "T_Entity",
                principalColumn: "entId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Activity_T_Jobs_actJobId",
                table: "T_Activity",
                column: "actJobId",
                principalTable: "T_Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
