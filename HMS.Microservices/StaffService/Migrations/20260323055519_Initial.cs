using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StaffService.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Staff_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Hotel management team", "Management" },
                    { 2, "Front desk and guest services", "Reception" },
                    { 3, "Room cleaning and maintenance", "Housekeeping" },
                    { 4, "Technical and facility maintenance", "Maintenance" },
                    { 5, "Hotel security personnel", "Security" }
                });

            migrationBuilder.InsertData(
                table: "Staff",
                columns: new[] { "Id", "DepartmentId", "Email", "FirstName", "HireDate", "IsActive", "LastName", "Phone", "Role" },
                values: new object[,]
                {
                    { 1, 1, "john.doe@hotel.com", "John", new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Doe", "123-456-7890", "Owner" },
                    { 2, 1, "jane.smith@hotel.com", "Jane", new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Smith", "123-456-7891", "Manager" },
                    { 3, 2, "bob.johnson@hotel.com", "Bob", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Johnson", "123-456-7892", "Receptionist" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Staff_DepartmentId",
                table: "Staff",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
