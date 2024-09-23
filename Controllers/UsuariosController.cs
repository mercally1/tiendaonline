using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tienda.Data;
using tienda.Models;

namespace tienda.Controllers
{
    public class UsuariosController : BaseController
    {
        public UsuariosController(OnlineShopDbContext context)
            :base(context){ }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var onlineShopDbContext = _context.usuarios.Include(u => u.Rol);
            return View(await onlineShopDbContext.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewData["RolId"] = new SelectList(_context.roles, "RolId", "Nombre");
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsuarioId,Nombre,Telefono,NombreUsuario,Contrasenia,Correo,Direccion,Cuidad,Departamento,CodigoPostal,RolId")] Usuario usuario)
        {
            var rol = await _context.roles
                .Where(d => d.RolId == usuario.RolId)
                .FirstOrDefaultAsync();

            if (rol != null)
            {
                usuario.Rol = rol;

                usuario.Direcciones = new List<Direccion>()
                {
                    new Direccion
                    {
                        Address= usuario.Direccion,
                        Cuidad = usuario.Cuidad,
                        Departamento = usuario.Departamento,
                        CodigoPostal = usuario.CodigoPostal
                    }
                };

                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RolId"] = new SelectList(_context.roles, "RolId", "Nombre", usuario.RolId);
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["RolId"] = new SelectList(_context.roles, "RolId", "Nombre", usuario.RolId);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioId,Nombre,Telefono,NombreUsuario,Contrasenia,Correo,Direccion,Cuidad,Departamento,CodigoPostal,RolId")] Usuario usuario)
        {
            if (id != usuario.UsuarioId)
            {
                return NotFound();
            }

            var rol = await _context.roles
                .Where(d => d.RolId == usuario.RolId)
                .FirstOrDefaultAsync();

            if (rol != null)
            {
                usuario.Rol = rol;

                var existingUser = await _context.usuarios
                    .Include(u => u.Direcciones)
                    .FirstOrDefaultAsync(u => u.UsuarioId == id);

                if (existingUser != null) 
                {
                    if (existingUser.Direcciones.Count > 0)
                    {
                        var direccion = existingUser.Direcciones.First();
                        direccion.Address = usuario.Direccion;
                        direccion.Cuidad = usuario.Cuidad;
                        direccion.Departamento = usuario.Departamento;
                        direccion.CodigoPostal = usuario.CodigoPostal;
                    }
                    else
                    {
                        existingUser.Direcciones = new List<Direccion>();
                        {
                            new Direccion
                            {
                                Address = usuario.Direccion,
                                Cuidad = usuario.Cuidad,
                                Departamento = usuario.Departamento,
                                CodigoPostal = usuario.CodigoPostal
                            };
                        }
                    }
                    // // try
                    // // {
                    // //     _context.Update(existingUser);
                    // //     await _context.SaveChanges();
                    // // }
                    // catch (DbUpdateConcurrencyException)
                    // {
                    //     if (!UsuarioExists(usuario.UsuarioId))
                    //     {
                    //         return NotFound();
                    //     }
                    //     else
                    //     {
                    //         throw;
                    //     }
                    // }
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["RolId"] = new SelectList(_context.roles, "RolId", "Nombre", usuario.RolId);
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.usuarios.Any(e => e.UsuarioId == id);
        }
    }
}
