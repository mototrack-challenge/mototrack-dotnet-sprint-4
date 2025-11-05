using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MT.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateTableAutenticacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MT_AUTENTICACAO",
                columns: table => new
                {
                    ID_USUARIO = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    EMAIL = table.Column<string>(type: "varchar2(150)", nullable: false),
                    NOME = table.Column<string>(type: "varchar2(150)", nullable: false),
                    SENHA = table.Column<string>(type: "varchar2(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MT_AUTENTICACAO", x => x.ID_USUARIO);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MT_AUTENTICACAO");
        }
    }
}
