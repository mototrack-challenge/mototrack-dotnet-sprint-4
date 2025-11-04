using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MT.Domain.Entities;

[Table("MT_COLABORADORES")]
[Index(nameof(Matricula), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
public class ColaboradorEntity
{
    [Key]
    [Column("ID_COLABORADOR")]
    public long Id { get; set; }

    [Required]
    [Column("NOME", TypeName = "varchar2(100)")]
    public string Nome { get; set; }

    [Required]
    [Column("MATRICULA", TypeName = "varchar2(9)")]
    public string Matricula { get; set; }

    [Column("EMAIL", TypeName = "varchar2(100)")]
    public string Email { get; set; }

    public ICollection<ServicoEntity> Servicos { get; set; } = new List<ServicoEntity>();
}
