using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using tienda.Data;

namespace tienda.Controllers;

public class BaseController : Controller
{
    public readonly OnlineShopDbContext _context;

    public BaseController (OnlineShopDbContext context) 
    {  
        _context = context; 
    }

    public override ViewResult View(string? viewName, object? model)
    {
        ViewBag.NumeroProductos = GetCarritoCount();
        return base.View(viewName, model);
    }

    protected int GetCarritoCount()
    {
        int Count = 0;

        string? carritoJson= Request.Cookies["carrito"];
        if (!string.IsNullOrEmpty(carritoJson))
        {
            var carrito = JsonConvert.DeserializeObject<List<ProductoIdAndCAntidad>>(carritoJson);
            if (carrito != null)
            {
                Count = carrito.Count;
            }
        }

        return Count;
    }
}
