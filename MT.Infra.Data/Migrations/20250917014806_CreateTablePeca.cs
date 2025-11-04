using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MT.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateTablePeca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MT_PECAS",
                columns: table => new
                {
                    ID_PECA = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "varchar2(100)", nullable: false),
                    CODIGO = table.Column<string>(type: "varchar2(10)", nullable: false),
                    DESCRICAO = table.Column<string>(type: "varchar2(100)", nullable: false),
                    QUANTIDADE_ESTOQUE = table.Column<int>(type: "number(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MT_PECAS", x => x.ID_PECA);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MT_PECAS");
        }
    }
}
