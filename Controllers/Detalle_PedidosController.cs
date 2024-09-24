using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tienda.Data;
using tienda.Models;

namespace tienda.Controllers
{
    public class Detalle_PedidosController : BaseController
    {
        public Detalle_PedidosController(OnlineShopDbContext context)
            : base(context){ }

        // GET: Detalle_Pedidos
        public async Task<IActionResult> Index()
        {
            var onlineShopDbContext = _context.detalle_Pedidos.Include(d => d.Producto).Include(d => d.pedidos);
            return View(await onlineShopDbContext.ToListAsync());
        }

        // GET: Detalle_Pedidos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalle_Pedido = await _context.detalle_Pedidos
                .Include(d => d.Producto)
                .Include(d => d.pedidos)
                .FirstOrDefaultAsync(m => m.Detalle_PedidoId == id);
            if (detalle_Pedido == null)
            {
                return NotFound();
            }

            return View(detalle_Pedido);
        }

        // GET: Detalle_Pedidos/Create
        public IActionResult Create()
        {
            ViewData["ProductoId"] = new SelectList(_context.productos, "ProductoId", "Codigo");
            ViewData["PedidoId"] = new SelectList(_context.pedidos, "PedidoId", "Estado");
            return View();
        }

        // POST: Detalle_Pedidos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Detalle_PedidoId,PedidoId,ProductoId,Cantidad,Precio")] Detalle_Pedido detalle_Pedido)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detalle_Pedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductoId"] = new SelectList(_context.productos, "ProductoId", "Codigo", detalle_Pedido.ProductoId);
            ViewData["PedidoId"] = new SelectList(_context.pedidos, "PedidoId", "Estado", detalle_Pedido.PedidoId);
            return View(detalle_Pedido);
        }

        // GET: Detalle_Pedidos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalle_Pedido = await _context.detalle_Pedidos.FindAsync(id);
            if (detalle_Pedido == null)
            {
                return NotFound();
            }
            ViewData["ProductoId"] = new SelectList(_context.productos, "ProductoId", "Codigo", detalle_Pedido.ProductoId);
            ViewData["PedidoId"] = new SelectList(_context.pedidos, "PedidoId", "Estado", detalle_Pedido.PedidoId);
            return View(detalle_Pedido);
        }

        // POST: Detalle_Pedidos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Detalle_PedidoId,PedidoId,ProductoId,Cantidad,Precio")] Detalle_Pedido detalle_Pedido)
        {
            if (id != detalle_Pedido.Detalle_PedidoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detalle_Pedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Detalle_PedidoExists(detalle_Pedido.Detalle_PedidoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductoId"] = new SelectList(_context.productos, "ProductoId", "Codigo", detalle_Pedido.ProductoId);
            ViewData["PedidoId"] = new SelectList(_context.pedidos, "PedidoId", "Estado", detalle_Pedido.PedidoId);
            return View(detalle_Pedido);
        }

        // GET: Detalle_Pedidos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalle_Pedido = await _context.detalle_Pedidos
                .Include(d => d.Producto)
                .Include(d => d.pedidos)
                .FirstOrDefaultAsync(m => m.Detalle_PedidoId == id);
            if (detalle_Pedido == null)
            {
                return NotFound();
            }

            return View(detalle_Pedido);
        }

        // POST: Detalle_Pedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detalle_Pedido = await _context.detalle_Pedidos.FindAsync(id);
            if (detalle_Pedido != null)
            {
                _context.detalle_Pedidos.Remove(detalle_Pedido);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Detalle_PedidoExists(int id)
        {
            return _context.detalle_Pedidos.Any(e => e.Detalle_PedidoId == id);
        }
    }
}
