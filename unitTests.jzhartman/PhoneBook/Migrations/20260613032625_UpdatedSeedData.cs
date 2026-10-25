using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PhoneBook.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Contacts",
                columns: new[] { "Id", "CategoryId", "Email", "FirstName", "LastName", "PhoneNumber" },
                values: new object[,]
                {
                    { 8, 1, "leafOnTheWind@serenity.com", "Hoban", "Washburne", "8888888" },
                    { 9, 1, "browncoats@serenity.com", "Zoe", "Washburne", "99999999" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 9);
        }
    }
}
