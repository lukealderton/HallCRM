using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJobServiceLinePrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "jbsQuantity",
                table: "T_JobService",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 1m);

            migrationBuilder.AddColumn<decimal>(
                name: "jbsUnitPrice",
                table: "T_JobService",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "jbsQuantity",
                table: "T_JobService");

            migrationBuilder.DropColumn(
                name: "jbsUnitPrice",
                table: "T_JobService");
        }
    }
}
