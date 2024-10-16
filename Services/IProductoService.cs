using tienda.Models;
using tienda.Models.ViewModels;

namespace tienda.Services;

public interface IProductoService
{
    Producto GetProducto(int id);

    Task <List<Producto>> GetProductoDestacados();

    Task<ProductosPaginadosViewModel> GetProductosPaginados( int? categoriaId, string? busqueda, int pagina, int productosPorPagina);

}
