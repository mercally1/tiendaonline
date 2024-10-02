using System.Data.Common;
using System.Security.Claims;
using System.Security.Principal;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using tienda.Data;
using tienda.Models;

namespace tienda.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(OnlineShopDbContext context) : base(context)
        {

        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(Usuario usuario)
        {
            try
            {
                if (usuario != null && ModelState.IsValid)
                {
                    if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == usuario.NombreUsuario))
                    {
                        ModelState.AddModelError(nameof(usuario.Nombre), "El nombre de usuario ya esta en uso");
                        return View(usuario);
                    }

                    var clienteRol = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre=="Cliente");

                    if (clienteRol != null)
                    {
                        usuario.RolId = clienteRol.RolId;
                    }

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

                    _context.Usuarios.Add(usuario);
                    await _context.SaveChangesAsync();

                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Name, usuario.NombreUsuario));
                    identity.AddClaim(new Claim(ClaimTypes.Role, "Cliente"));

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                    return RedirectToAction("Index", "Home");
                }
                return View(usuario);  
            }
            catch (DbException dbException)
            {
                return HandleDbError(dbException);
            }

            // return View();
        }
    
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login(string username, string password)
        {
            try
            {
                var user = await _context.Usuarios.FirstOrDefaultAsync(u=>u.NombreUsuario==username && u.Contrasenia==password); 
                
                if (user != null)
                {
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Name, username));
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.UsuarioId.ToString()));

                    var rol = await _context.Roles.FirstOrDefaultAsync(r=>r.RolId == user.RolId);
                    
                    if (rol!=null)
                    {
                       identity.AddClaim(new Claim(ClaimTypes.Role, rol.Nombre));
                    }
                    
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                    if (rol != null) {
                        if (rol.Nombre == "Administrador" || rol.Nombre == "Staff")
                            return RedirectToAction("Index", "Dashboard");
                        else
                            return RedirectToAction("Index", "Home");
                    }
                }
               
                ModelState.AddModelError("", "Credenciales Invalidas.");
                return View();
            }
            catch (Exception e)
            {
                return (ActionResult)HandleError(e);
            }
        }

        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            return View();
        }

    }
}



