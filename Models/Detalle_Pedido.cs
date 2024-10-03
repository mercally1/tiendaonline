using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tienda.Models;

public class Detalle_Pedido
{
    [Key]
    public int Detalle_PedidoId { get; set; }

    [Required]
    public int PedidoId { get; set; }

    [ForeignKey("PedidoId")]
    public virtual Pedido Pedidos { get; set; } = null!;

    [Required]
    public int ProductoId { get; set; }

    [ForeignKey("ProductoId")]
    public virtual Producto Producto { get; set; } = null!;

    [Required]
    public int Cantidad { get; set; }

    [Required]
    public decimal Precio { get; set; }
}
