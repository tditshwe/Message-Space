using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MessageApi.Database.Migrations
{
    /// <inheritdoc />
    public partial class SeedAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "Username", "ImageUrl", "Name", "Password", "Role", "Status" },
                values: new object[,]
                {
                    { "archi", null, "Archibaldo", "AQAAAAIAAYagAAAAEINoKBPVubTytHlU50RzwhBqlNLoL19huAKwccvxEUOQaYE8G+inmgJoCT7bpTxEwA==", "User", "Ready to chat" },
                    { "moriarty", null, "Moriarty", "AQAAAAIAAYagAAAAEIOYqTqM5ZBa8nGF6q+lNS6v2qwXBAafjIn0ZedetP546tI1GqLT6mD5A/rqRHssAA==", "User", "Ready to chat" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "Username",
                keyValue: "archi");

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "Username",
                keyValue: "moriarty");
        }
    }
}
