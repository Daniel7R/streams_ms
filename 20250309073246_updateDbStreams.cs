using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StreamsMS.Migrations
{
    /// <inheritdoc />
    public partial class updateDbStreams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "platforms",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "YouTube" },
                    { 2, "Twitch" },
                    { 3, "Zoom" },
                    { 4, "Meet" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "platforms",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "platforms",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "platforms",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "platforms",
                keyColumn: "id",
                keyValue: 4);
        }
    }
}
