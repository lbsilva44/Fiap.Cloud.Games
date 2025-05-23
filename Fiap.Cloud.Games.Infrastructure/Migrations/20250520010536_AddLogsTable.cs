using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fiap.Cloud.Games.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLogsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
    name: "Logs",
    columns: table => new
    {
        Id = table.Column<int>(nullable: false)
            .Annotation("SqlServer:Identity", "1, 1"),
        Timestamp = table.Column<DateTimeOffset>(nullable: false),
        Level = table.Column<string>(maxLength: 50, nullable: true),
        Message = table.Column<string>(nullable: true),
        Exception = table.Column<string>(nullable: true),
        Properties = table.Column<string>(nullable: true)
    },
    constraints: table =>
    { table.PrimaryKey("PK_Logs", x => x.Id); });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.DropTable(name: "Logs");
        }
    }
}
