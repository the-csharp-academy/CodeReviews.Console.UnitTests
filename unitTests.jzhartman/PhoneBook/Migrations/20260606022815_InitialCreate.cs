using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PhoneBook.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contacts_ContactCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ContactCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ContactCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Uncategorized" },
                    { 2, "Family" },
                    { 3, "Friends" },
                    { 4, "Work" }
                });

            migrationBuilder.InsertData(
                table: "Contacts",
                columns: new[] { "Id", "CategoryId", "Email", "FirstName", "LastName", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, 1, "browncoat@serenity.com", "Malcolm", "Reynolds", "1111111111" },
                    { 2, 1, "companion@serenity.com", "Inara", "Serra", "2222222222" },
                    { 3, 1, "book@serenity.com", "Shepherd", "Book", "3333333333" },
                    { 4, 1, "vera@serenity.com", "Jayne", "Cobb", "4444444444" },
                    { 5, 1, "mechanic@serenity.com", "Kaylee", "Frye", "5555555555" },
                    { 6, 1, "awesomeDoctor@serenity.com", "Simon", "Tam", "6666666666" },
                    { 7, 1, "miranda@serenity.com", "River", "Tam", "7777777777" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_CategoryId",
                table: "Contacts",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "ContactCategories");
        }
    }
}
