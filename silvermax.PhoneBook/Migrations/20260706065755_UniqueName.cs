using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace silvermax.PhoneBook.Migrations
{
    /// <inheritdoc />
    public partial class UniqueName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Contacts_Name",
                table: "Contacts",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contacts_Name",
                table: "Contacts");
        }
    }
}
