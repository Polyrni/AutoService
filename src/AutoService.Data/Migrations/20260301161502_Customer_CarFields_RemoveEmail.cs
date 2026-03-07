using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoService.Data.Migrations
{
    /// <inheritdoc />
    public partial class Customer_CarFields_RemoveEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Customers",
                newName: "LicensePlate");

            migrationBuilder.AddColumn<string>(
                name: "CarBrand",
                table: "Customers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarBrand",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "LicensePlate",
                table: "Customers",
                newName: "Email");
        }
    }
}
