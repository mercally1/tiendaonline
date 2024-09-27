namespace tienda.Models.ViewModels;

public class ProcederConCompraViewModel
{
    public CarritoItemViewModel Carrito { get; set; } = null!;

    public List<Direccion> direcciones { get; set; } = null!;

    public int DireccionSeleccionada { get; set; }
}
