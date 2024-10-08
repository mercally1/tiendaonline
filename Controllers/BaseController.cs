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
            var carrito = JsonConvert.DeserializeObject<List<ProductoIdAndCantidad>>(carritoJson);
            if (carrito != null)
            {
                Count = carrito.Count;
            }
        }

        return Count;
    }

    public async Task<CarritoViewModel> AgregarProductoAlCarrito(int productoId, int cantidad)
    {
        var producto  = await _context.Productos.FindAsync(productoId);

        if (producto != null)
        {
            var carritoViewModel = await GetCarritoViewModelAsync();

            var carritoItem = carritoViewModel.Item.FirstOrDefault
                (item => item.ProductoId == productoId);

            if (carritoItem != null)
                carritoItem.Cantidad += cantidad;
            else
                carritoViewModel.Item.Add(
                    new CarritoItemViewModel
                    {
                        ProductoId = producto.ProductoId,
                        Nombre = producto.Nombre,
                        Precio = producto.Precio,
                        Cantidad = cantidad
                    }
                );

            carritoViewModel.Total = carritoViewModel.Item.Sum(
                item => item.Cantidad * item.Precio
            );

            await UpdateCarritoViewModelAsync(carritoViewModel);

            return carritoViewModel;
        }

        return new CarritoViewModel();
    }

    public async Task UpdateCarritoViewModelAsync(CarritoViewModel carritoViewModel)
    {
        var productoIds = carritoViewModel.Item.Select(
            item => new ProductoIdAndCantidad
            {
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad 
            })
            .ToList();

        var carritoJson =await Task.Run(() => JsonConvert.SerializeObject(productoIds));
        Response.Cookies.Append(
            "carrito", 
            carritoJson,
            new CookieOptions{ Expires = DateTimeOffset.Now.AddDays(7)}
        );
    }

    public async Task<CarritoViewModel> GetCarritoViewModelAsync()
    {
        var carritoJson = Request.Cookies["carrito"];

        if(string.IsNullOrEmpty(carritoJson))
            
            return new CarritoViewModel();

        var ProductoIdAndCantidad = JsonConvert.DeserializeObject<List<ProductoIdAndCantidad>>(carritoJson);

        var carritoViewModel = new CarritoViewModel();

        if(ProductoIdAndCantidad != null){

            foreach(var item in ProductoIdAndCantidad)
            {
                var producto = await _context.Productos.FindAsync(item.ProductoId);

                if(producto != null)
                {
                    carritoViewModel.Item.Add(
                        new CarritoItemViewModel
                        {
                            ProductoId = producto.ProductoId,
                            Nombre = producto.Nombre,
                            Precio = producto.Precio,
                            Cantidad = item.Cantidad
                        }
                    );
                }
            }
        }
        carritoViewModel.Total = carritoViewModel.Item.Sum(item => item.Subtotal);

        return new CarritoViewModel();
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
