using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fiap.Cloud.Games.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJogoIdToMovimentoCarteira : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JogoId",
                table: "CarteiraMovimentos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarteiraMovimentos_JogoId",
                table: "CarteiraMovimentos",
                column: "JogoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CarteiraMovimentos_JogoId",
                table: "CarteiraMovimentos");

            migrationBuilder.DropColumn(
                name: "JogoId",
                table: "CarteiraMovimentos");
        }
    }
}
