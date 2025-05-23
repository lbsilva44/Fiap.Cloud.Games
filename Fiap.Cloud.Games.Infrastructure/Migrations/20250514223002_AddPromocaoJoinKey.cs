using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fiap.Cloud.Games.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPromocaoJoinKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Biblioteca_Jogo_JogoId",
                table: "Biblioteca");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuario",
                table: "Usuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Jogo",
                table: "Jogo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Biblioteca",
                table: "Biblioteca");

            migrationBuilder.RenameTable(
                name: "Usuario",
                newName: "Usuarios");

            migrationBuilder.RenameTable(
                name: "Jogo",
                newName: "Jogos");

            migrationBuilder.RenameTable(
                name: "Biblioteca",
                newName: "Bibliotecas");

            migrationBuilder.RenameIndex(
                name: "IX_Biblioteca_JogoId",
                table: "Bibliotecas",
                newName: "IX_Bibliotecas_JogoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Jogos",
                table: "Jogos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bibliotecas",
                table: "Bibliotecas",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Promocoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescontoPercentual = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ativa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promocoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromocaoJogos",
                columns: table => new
                {
                    PromocaoId = table.Column<int>(type: "int", nullable: false),
                    JogoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromocaoJogos", x => new { x.PromocaoId, x.JogoId });
                    table.ForeignKey(
                        name: "FK_PromocaoJogos_Jogos_JogoId",
                        column: x => x.JogoId,
                        principalTable: "Jogos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromocaoJogos_Promocoes_PromocaoId",
                        column: x => x.PromocaoId,
                        principalTable: "Promocoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromocaoJogos_JogoId",
                table: "PromocaoJogos",
                column: "JogoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bibliotecas_Jogos_JogoId",
                table: "Bibliotecas",
                column: "JogoId",
                principalTable: "Jogos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bibliotecas_Jogos_JogoId",
                table: "Bibliotecas");

            migrationBuilder.DropTable(
                name: "PromocaoJogos");

            migrationBuilder.DropTable(
                name: "Promocoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Jogos",
                table: "Jogos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bibliotecas",
                table: "Bibliotecas");

            migrationBuilder.RenameTable(
                name: "Usuarios",
                newName: "Usuario");

            migrationBuilder.RenameTable(
                name: "Jogos",
                newName: "Jogo");

            migrationBuilder.RenameTable(
                name: "Bibliotecas",
                newName: "Biblioteca");

            migrationBuilder.RenameIndex(
                name: "IX_Bibliotecas_JogoId",
                table: "Biblioteca",
                newName: "IX_Biblioteca_JogoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuario",
                table: "Usuario",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Jogo",
                table: "Jogo",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Biblioteca",
                table: "Biblioteca",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Biblioteca_Jogo_JogoId",
                table: "Biblioteca",
                column: "JogoId",
                principalTable: "Jogo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
