using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tienda.Models;

public class Pedido
{
    [Key]
    public int PedidoId { get; set; }

    [Required]
    public int UsuarioId { get; set;} 

    [ForeignKey("UsuarioId")]
    public virtual Usuario Usuarios { get; set; } = null!;

    [Required]
    public DateTime Fecha { get; set; }
    
    [Required]
    public string Estado { get; set; } = null!;

    [Required]
    public int DireccionSeleccionada { get; set;} 

    [ForeignKey("DireccionId")]
    public virtual Direccion Direccion { get; set; } = null!;

    public decimal Total { get; set; }

    public ICollection<Detalle_Pedido> DetallePedidos { get; set; } = null!;
}
