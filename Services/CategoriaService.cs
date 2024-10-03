using Microsoft.EntityFrameworkCore;
using tienda.Data;
using tienda.Models;

namespace tienda.Services;

public class CategoriaService : ICategoriaService
{
    private readonly OnlineShopDbContext _context;

    public CategoriaService(OnlineShopDbContext context)
    {
        _context = context;
    }

    public async Task<List<Categoria>> GetCategorias()
    {
        return await _context.Categorias.ToListAsync();
    }
}
