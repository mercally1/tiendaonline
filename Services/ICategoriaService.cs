using tienda.Models;

namespace tienda.Services;

public interface ICategoriaService
{
    Task<List<Categoria>> GetCategorias();
}
