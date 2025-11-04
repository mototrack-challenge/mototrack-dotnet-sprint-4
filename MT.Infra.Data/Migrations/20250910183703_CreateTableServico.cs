using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MT.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateTableServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MT_SERVICOS",
                columns: table => new
                {
                    ID_SERVICO = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DESCRICAO = table.Column<string>(type: "varchar2(100)", nullable: false),
                    DATA_CADASTRO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    STATUS = table.Column<string>(type: "varchar2(20)", nullable: false),
                    ID_MOTO = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ID_COLABORADOR = table.Column<long>(type: "NUMBER(19)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MT_SERVICOS", x => x.ID_SERVICO);
                    table.ForeignKey(
                        name: "FK_MT_SERVICOS_MT_COLABORADORES_ID_COLABORADOR",
                        column: x => x.ID_COLABORADOR,
                        principalTable: "MT_COLABORADORES",
                        principalColumn: "ID_COLABORADOR",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MT_SERVICOS_MT_MOTOS_ID_MOTO",
                        column: x => x.ID_MOTO,
                        principalTable: "MT_MOTOS",
                        principalColumn: "ID_MOTO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MT_SERVICOS_ID_COLABORADOR",
                table: "MT_SERVICOS",
                column: "ID_COLABORADOR");

            migrationBuilder.CreateIndex(
                name: "IX_MT_SERVICOS_ID_MOTO",
                table: "MT_SERVICOS",
                column: "ID_MOTO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MT_SERVICOS");
        }
    }
}
