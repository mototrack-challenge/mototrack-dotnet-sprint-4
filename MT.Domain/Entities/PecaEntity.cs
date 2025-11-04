using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MT.Domain.Entities;

[Table("MT_PECAS")]
public class PecaEntity
{
    [Key]
    [Column("ID_PECA")]
    public long Id { get; set; }

    [Required]
    [Column("NOME", TypeName = "varchar2(100)")]
    public string Nome { get; set; }

    [Required]
    [Column("CODIGO", TypeName = "varchar2(10)")]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [Column("DESCRICAO", TypeName = "varchar2(100)")]
    public string? Descricao { get; set; }

    [Required]
    [Column("QUANTIDADE_ESTOQUE", TypeName = "number(10)")]
    public int QuantidadeEstoque { get; set; }
}
