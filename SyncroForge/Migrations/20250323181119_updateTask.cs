using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SyncroForge.Migrations
{
    /// <inheritdoc />
    public partial class updateTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Employees_CreatedById",
                table: "Tasks");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_CreatedById",
                table: "Tasks",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_CreatedById",
                table: "Tasks");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Employees_CreatedById",
                table: "Tasks",
                column: "CreatedById",
                principalTable: "Employees",
                principalColumn: "Id");
        }
    }
}
