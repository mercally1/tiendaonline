using tienda.Models;
using tienda.Models.ViewModels;

namespace tienda.Services;

public interface IProductoService
{
    Producto GetProducto(int id);

    Task <List<Producto>> GetProductoDestacados();

    Task<ProductosPaginadosViewModel> GetProductoPaginados( int? categoriaId, string? busqueda, int pagijna, int Productos);

}
