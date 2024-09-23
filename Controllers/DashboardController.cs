using Microsoft.AspNetCore.Mvc;
using tienda.Data;

namespace tienda.Controllers;

public class DashboardController : BaseController
{
    public DashboardController(OnlineShopDbContext context) : base(context)
    {
    }

    public IActionResult Index()
    {
        return View();
    }
}
