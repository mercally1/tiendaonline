using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tienda.Models;

public class Direccion
{   
    [Key]
    public int DireccionId { get; set; }

    [Required, StringLength(50)]
    public string Address { get; set; } = null!;

    [Required, StringLength(50)]
    public string Cuidad { get; set; } = null!;

    [Required, StringLength(50)]
    public string Departamento { get; set; } = null!;

    [Required, StringLength(10)]
    public string CodigoPostal { get; set; } = null!;

    [Required]
    public int UsuarioId { get; set; }

    [ForeignKey("UsuarioId")]
    public virtual Usuario Usuarios { get; set; } = null!;
}
