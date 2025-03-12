using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamsMS.Migrations
{
    /// <inheritdoc />
    public partial class fixForeign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_streams_id_platform",
                table: "streams");

            migrationBuilder.CreateIndex(
                name: "IX_streams_id_platform",
                table: "streams",
                column: "id_platform");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_streams_id_platform",
                table: "streams");

            migrationBuilder.CreateIndex(
                name: "IX_streams_id_platform",
                table: "streams",
                column: "id_platform",
                unique: true);
        }
    }
}
