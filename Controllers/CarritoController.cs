using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;
using tienda.Data;
using tienda.Models;
using tienda.Models.ViewModels;

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

            foreach (var item in carritoViewModel.Items)
            {
                var producto = await _context.Productos.FindAsync(item.ProductoId);
                if(producto != null)
                {
                    item.Producto = producto;

                    if(!producto.Activo)
                        carritoViewModel.Items.Remove(item);
                    else
                        item.Cantidad = Math.Min(item.Cantidad, producto.Stock);
                }
                else 
                    item.Cantidad = 0;
            }
            carritoViewModel.Total = carritoViewModel.Items.Sum(item => item.Subtotal);

            var usuarioId = User.Identity?.IsAuthenticated == true ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)):0;
            
            var direcciones = User.Identity?.IsAuthenticated == true ? 
                _context.Direcciones.Where(d => d.UsuarioId == usuarioId). ToList() : new List<Direccion>();

            var procederConCompraViewModel = new ProcederConCompraViewModel
            {
                Carrito = carritoViewModel,
                Direcciones = direcciones
            };
            return View(procederConCompraViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarCantidad(int id, int cantidad)
        {
            var carritoViewModel = await GetCarritoViewModelAsync();
            var carritoItem = carritoViewModel.Items.FirstOrDefault(d => d.ProductoId == id);

            if (carritoItem != null)
            {
                carritoItem.Cantidad = cantidad;
                var producto = await _context.Productos.FindAsync(id);
                if (producto != null && producto.Activo && producto.Stock > 0)
                    carritoItem.Cantidad = Math.Min(cantidad, producto.Stock);

                await UpdateCarritoViewModelAsync(carritoViewModel);
            }
            return RedirectToAction("Index", "Carrito");
        }

        [HttpPost]
         public async Task<IActionResult> EliminarProducto(int id, int cantidad)
        {
            var carritoViewModel = await GetCarritoViewModelAsync();
            var carritoItem = carritoViewModel.Items.FirstOrDefault(d => d.ProductoId == id);

            if (carritoItem != null)
            {
               carritoViewModel.Items.Remove(carritoItem);

                await UpdateCarritoViewModelAsync(carritoViewModel);
            }
            return RedirectToAction("Index");
        }

        [HttpPost] 
        public async Task<IActionResult> VaciarCarrito()
        {
            await RemoveCarritoViewModelAsync();
            return RedirectToAction("Index");
        }

        private async Task RemoveCarritoViewModelAsync()
        {
            await Task.Run(() => Response.Cookies.Delete("carrito"));
        }

        private readonly string clienteId = "";

        private readonly string clienteSecret = "";

        public IActionResult ProcederConCompra(decimal montoTotal, int direccionIdSeleccionada)
        {
            if (direccionIdSeleccionada > 0)
            {
                Response.Cookies.Append
                    ("direccionseleccionada", direccionIdSeleccionada.ToString(),
                    new CookieOptions {Expires = DateTimeOffset.Now.AddDays(1)});
            }else
                return View("Index");

            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(BuildRequestBody(montoTotal));

            var environment = new SandboxEnvironment(clienteId, clienteSecret);
            var client =new PayPalHttpClient(environment);

            try
            {
                var response = client.Execute(request).Result;
                var statusCode = response.StatusCode;
                var responseBody = response.Result<Order>();

                var approveLink = responseBody.Links.FirstOrDefault(x=>x.Rel=="approve");
                if(approveLink!=null)
                return Redirect(approveLink.Href);
                else
                   return RedirectToAction("Error");
            }
            catch (HttpException e)
            {
                return (IActionResult)e;
            }
        }

        //metodo para proceder con la compra
        private OrderRequest BuildRequestBody(decimal montoTotal)
        {
            var baseUrl=$"{Request.Scheme}://{Request.Host}";

            var request = new OrderRequest(){
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>()
                {
                    new PurchaseUnitRequest()
                    {
                        AmountWithBreakdown= new AmountWithBreakdown()
                        {
                            CurrencyCode = "USD",
                            Value =montoTotal.ToString("F2")
                        }
                    }
                },
                ApplicationContext = new ApplicationContext()
                {
                    ReturnUrl=$"{baseUrl} /Carrito/PagoCompletado",
                    CancelUrl=$"{baseUrl} /Carrito/Index"
                }
            };
            return request;
        }

        public IActionResult PagoCompletado()
        {
            try
            {
                var carritoJson = Request.Cookies["carrito"];

                int direccionId = 0;

                if (Request.Cookies.TryGetValue("direccionseleccionada", out string? cookieValue) && int.TryParse(cookieValue, out int parseValue))
                {
                    direccionId = parseValue;
                }
                List<ProductoIdAndCantidad>? productoIdAndCantidads = !string.IsNullOrEmpty(carritoJson)? JsonConvert
                .DeserializeObject<List<ProductoIdAndCantidad>>(carritoJson): null;

                CarritoViewModel carritoViewModel = new();

                if(productoIdAndCantidads!=null)
                {
                    foreach(var item in productoIdAndCantidads)
                    {
                        var producto = _context.Productos.Find(item.ProductoId);
                        if(producto != null)
                        {
                            carritoViewModel.Items.Add
                            (
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

                var usuarioId = User.Identity?.IsAuthenticated == true ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) : 0;

                carritoViewModel.Total = carritoViewModel.Items.Sum(i => i.Subtotal);

                var pedido  = new Pedido
                {
                    UsuarioId = usuarioId,
                    Fecha = DateTime.UtcNow,
                    Estado = "Vendido",
                    DireccionSeleccionada = direccionId,
                    Total = carritoViewModel.Total
                };

                _context.Pedidos.Add(pedido);
                _context.SaveChanges();

                foreach (var item in carritoViewModel.Items)
                {
                    var pedidoDetalle = new Detalle_Pedido{
                        PedidoId = pedido.PedidoId,
                        ProductoId = item.ProductoId,
                        Cantidad = item.Cantidad,
                        Precio = item.Precio
                    };

                    _context.DetallePedidos.Add(pedidoDetalle);

                    var producto = _context.Productos.FirstOrDefault(p => p.ProductoId== item.ProductoId);

                    if(producto != null)
                    producto.Stock -= item.Cantidad;
                }
                _context.SaveChanges();

                Response.Cookies.Delete("carrito");
                Response.Cookies.Delete("direccionseleccionada");

                ViewBag.DetallePedidos = _context.DetallePedidos
                .Where(dp => dp.PedidoId == pedido.PedidoId)
                .Include(dp => dp.Producto)
                .ToList();

                return View("PagoCompletado", pedido);

            }
            catch (Exception e)
            {
                return HandleError (e);
            }
        }
    }
}