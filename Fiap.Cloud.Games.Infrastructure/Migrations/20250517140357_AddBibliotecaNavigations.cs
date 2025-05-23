using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fiap.Cloud.Games.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBibliotecaNavigations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bibliotecas_Jogos_JogoId",
                table: "Bibliotecas");

            migrationBuilder.CreateIndex(
                name: "IX_Bibliotecas_UsuarioId",
                table: "Bibliotecas",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bibliotecas_Jogos_JogoId",
                table: "Bibliotecas",
                column: "JogoId",
                principalTable: "Jogos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bibliotecas_Usuarios_UsuarioId",
                table: "Bibliotecas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bibliotecas_Jogos_JogoId",
                table: "Bibliotecas");

            migrationBuilder.DropForeignKey(
                name: "FK_Bibliotecas_Usuarios_UsuarioId",
                table: "Bibliotecas");

            migrationBuilder.DropIndex(
                name: "IX_Bibliotecas_UsuarioId",
                table: "Bibliotecas");

            migrationBuilder.AddForeignKey(
                name: "FK_Bibliotecas_Jogos_JogoId",
                table: "Bibliotecas",
                column: "JogoId",
                principalTable: "Jogos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
