using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MT.Domain.Enums;

namespace MT.Domain.Entities;

[Table("MT_SERVICOS")]
public class ServicoEntity
{
    [Key]
    [Column("ID_SERVICO")]
    public long Id { get; set; }

    [Required]
    [Column("DESCRICAO", TypeName = "varchar2(100)")]
    public string Descricao { get; set; }

    [Required]
    [Column("DATA_CADASTRO")]
    public DateTime DataCadastro { get; set; } = DateTime.Now;

    [Required]
    [Column("STATUS", TypeName = "varchar2(20)")]
    public StatusServico Status { get; set; } = StatusServico.Pendente;

    [Required]
    [Column("ID_MOTO")]
    public long MotoId { get; set; }

    [ForeignKey("MotoId")]
    [JsonIgnore]
    public MotoEntity Moto { get; set; }

    [Required]
    [Column("ID_COLABORADOR")]
    public long ColaboradorId { get; set; }

    [ForeignKey("ColaboradorId")]
    [JsonIgnore]
    public ColaboradorEntity Colaborador { get; set; }
}
