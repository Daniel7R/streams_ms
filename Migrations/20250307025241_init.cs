using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StreamsMS.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "platforms",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_platforms", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "streams",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_match = table.Column<int>(type: "integer", nullable: false),
                    id_platform = table.Column<int>(type: "integer", nullable: false),
                    PlatformId = table.Column<int>(type: "integer", nullable: false),
                    url_stream = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_streams", x => x.id);
                    table.ForeignKey(
                        name: "FK_streams_platforms_PlatformId",
                        column: x => x.PlatformId,
                        principalTable: "platforms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_streams_platforms_id_platform",
                        column: x => x.id_platform,
                        principalTable: "platforms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_platforms_name",
                table: "platforms",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_streams_id_match_id_platform",
                table: "streams",
                columns: new[] { "id_match", "id_platform" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_streams_id_platform",
                table: "streams",
                column: "id_platform",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_streams_PlatformId",
                table: "streams",
                column: "PlatformId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "streams");

            migrationBuilder.DropTable(
                name: "platforms");
        }
    }
}
