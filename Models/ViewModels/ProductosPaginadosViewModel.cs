namespace tienda.Models.ViewModels;

public class ProductosPaginadosViewModel
{
    public List<Producto> productos { get; set; } = null!;

    public int PaginaActual { get; set; }

    public int TotalPaginas { get; set; }

    public int? CategoriaIdSeleccionada { get; set; }

    public string? Busqueda { get; set; }

    public bool MostarMensajeSinResultado { get; set; }

    public string? NombreCategoriaSeleccionada { get; set; }
}
