using System.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using tienda.Data;
using tienda.Models;

namespace tienda.Controllers
{
    public class CarritoController : BaseController
    {
         public CarritoController(OnlineShopDbContext context) 
         : base(context) { }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var carritoViewModel = await GetCarritoViewModelAsync();

            foreach (var item in carritoViewModel.Item) 
            {
            
                var producto = await _context.productos.FindAsync(item.ProductoId);
                if (producto != null)
                {
                    item.Producto = producto;

                    if (!producto.Activo)
                        item.Cantidad = 0;
                    else
                        item.Cantidad = Math.Min(item.Cantidad, producto.Stock);

                    if (item.Cantidad == 0)
                        item.Cantidad = 1;
                }
                else
                item.Cantidad = 0;
            }

            var UsuarioId=User.Identity?.IsAuthenticated==true ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) : 0;

            var direcciones = User.Identity?.IsAuthenticated == true ?
                _context.direccions.Where(d => d.UsuarioId == UsuarioId).ToList() : new List<Direccion>();

            var procederConCompraViewModel = new Models.ViewModels.ProcederConCompraViewModel
            {
                Carrito = carritoViewModel,
                Direcciones = direcciones
            };

            return View(procederConCompraViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> ActualizarCantidad(int id, int cantidad)
        {
            var carritoViewModel = await GetCarritoViewModelAsync();
            var carritoItem = carritoViewModel.Item.FirstOrDefault(d => d.ProductoId == id);

            if (carritoItem != null)
            {
                carritoItem.Cantidad = cantidad;
                var producto = await _context.productos.FindAsync(id);
                if (producto != null && producto.Activo && producto.Stock > 0)
                    carritoItem.Cantidad =Math.Min(cantidad,producto.Stock);

                await UpdateCarritoViewModelAsync(carritoViewModel);
            }
            
            return RedirectToAction("Index", "Carrito");
        }
    
    }
   
}