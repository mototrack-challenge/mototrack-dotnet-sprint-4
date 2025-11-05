using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MT.Domain.Entities;

[Table("MT_AUTENTICACAO")]
public class UsuarioEntity
{
    [Key]
    [Column("ID_USUARIO")]
    public long Id { get; set; }

    [Required(ErrorMessage = "O campo Email é obrigatorio!")]
    [Column("EMAIL", TypeName = "varchar2(150)")]
    public string Email { get; set; }

    [Required(ErrorMessage = "O campo Nome é obrigatorio!")]
    [Column("NOME", TypeName = "varchar2(150)")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O campo Senha é obrigatorio!")]
    [Column("SENHA", TypeName = "varchar2(150)")]
    public string Senha { get; set; }
}
