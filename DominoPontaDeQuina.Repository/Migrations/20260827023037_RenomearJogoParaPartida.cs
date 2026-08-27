using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DominoPontaDeQuina.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RenomearJogoParaPartida : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(name: "Jogos", newName: "Partidas");
            migrationBuilder.RenameTable(name: "ParticipacoesJogo", newName: "ParticipacoesPartida");
            migrationBuilder.RenameColumn(
                name: "JogoId",
                table: "ParticipacoesPartida",
                newName: "PartidaId");

            migrationBuilder.Sql("""
                UPDATE Partidas
                SET Status = CASE Status
                    WHEN 0 THEN 'Aguardando'
                    WHEN 1 THEN 'EmAndamento'
                    WHEN 2 THEN 'Finalizado'
                    WHEN 3 THEN 'Cancelado'
                    ELSE 'Aguardando'
                END
                """);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Partidas",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.DropIndex(
                name: "IX_ParticipacoesJogo_JogadorId",
                table: "ParticipacoesPartida");
            migrationBuilder.DropIndex(
                name: "IX_ParticipacoesJogo_JogoId",
                table: "ParticipacoesPartida");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipacoesPartida_JogadorId",
                table: "ParticipacoesPartida",
                column: "JogadorId");
            migrationBuilder.CreateIndex(
                name: "IX_ParticipacoesPartida_PartidaId",
                table: "ParticipacoesPartida",
                column: "PartidaId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParticipacoesPartida_PartidaId_JogadorId",
                table: "ParticipacoesPartida",
                columns: new[] { "PartidaId", "JogadorId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ParticipacoesPartida_PartidaId_JogadorId",
                table: "ParticipacoesPartida");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_ParticipacoesPartida_JogadorId",
                table: "ParticipacoesPartida");
            migrationBuilder.DropIndex(
                name: "IX_ParticipacoesPartida_PartidaId",
                table: "ParticipacoesPartida");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipacoesJogo_JogadorId",
                table: "ParticipacoesPartida",
                column: "JogadorId");
            migrationBuilder.CreateIndex(
                name: "IX_ParticipacoesJogo_JogoId",
                table: "ParticipacoesPartida",
                column: "JogoId");

            migrationBuilder.Sql("""
                UPDATE Partidas
                SET Status = CASE Status
                    WHEN 'Aguardando' THEN 0
                    WHEN 'EmAndamento' THEN 1
                    WHEN 'Finalizado' THEN 2
                    WHEN 'Cancelado' THEN 3
                    ELSE 0
                END
                """);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Partidas",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.RenameColumn(
                name: "PartidaId",
                table: "ParticipacoesPartida",
                newName: "JogoId");
            migrationBuilder.RenameTable(name: "ParticipacoesPartida", newName: "ParticipacoesJogo");
            migrationBuilder.RenameTable(name: "Partidas", newName: "Jogos");
        }
    }
}
