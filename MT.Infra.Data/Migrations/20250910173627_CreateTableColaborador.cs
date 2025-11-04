using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MT.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateTableColaborador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MT_COLABORADORES",
                columns: table => new
                {
                    ID_COLABORADOR = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "varchar2(100)", nullable: false),
                    MATRICULA = table.Column<string>(type: "varchar2(9)", nullable: false),
                    EMAIL = table.Column<string>(type: "varchar2(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MT_COLABORADORES", x => x.ID_COLABORADOR);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MT_COLABORADORES_EMAIL",
                table: "MT_COLABORADORES",
                column: "EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MT_COLABORADORES_MATRICULA",
                table: "MT_COLABORADORES",
                column: "MATRICULA",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MT_COLABORADORES");
        }
    }
}
