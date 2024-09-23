using System.Data.Common;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using tienda.Data;
using tienda.Models;
using tienda.Models.ViewModels;

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

    protected IActionResult HandleError(Exception e)
    {
        return View(
            "Error", new ErrorViewModel{
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            }
        );
    }

    protected IActionResult HandleDbError(DbException dbException)
    {
        var ViewModel = new DbErrorViewModel{
            ErrorMessage = "Error de la base de Datos",
            Details = dbException.Message
        };

        return View("DbError", ViewModel);
    }

     protected IActionResult HandleDbUpdateError(DbUpdateException dbUpdateException)
    {
        var ViewModel = new DbErrorViewModel{
            ErrorMessage = "!Hubo un error de actualización en la base de Datos",
            Details = dbUpdateException.Message
        };

        return View("DbError", ViewModel);
    }
}
