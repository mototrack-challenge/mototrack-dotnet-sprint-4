using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MT.Domain.Enums;

namespace MT.Domain.Entities;

[Table("MT_MOTOS")]
public class MotoEntity
{
    [Key]
    [Column("ID_MOTO")]
    public long Id { get; set; }

    [Required(ErrorMessage = "O campo Placa é obrigatorio!")]
    [StringLength(7, ErrorMessage = "A Placa deve conter exatamente 7 caracteres.")]
    [Column("PLACA")]
    public string Placa { get; set; }

    [Required(ErrorMessage = "O campo Chassi é obrigatorio!")]
    [StringLength(17, ErrorMessage = "O Chassi deve conter exatamente 17 caracteres.")]
    [Column("CHASSI")]
    public string Chassi { get; set; }

    [Required(ErrorMessage = "O campo Modelo é obrigatorio!")]
    [Column("MODELO", TypeName = "varchar2(11)")]
    public ModeloMoto Modelo { get; set; }

    [Required(ErrorMessage = "O campo Status é obrigatorio!")]
    [Column("STATUS", TypeName = "varchar2(30)")]
    public StatusMoto Status { get; set; }

    public ICollection<ServicoEntity> Servicos { get; set; } = new List<ServicoEntity>();
}
