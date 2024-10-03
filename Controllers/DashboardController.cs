using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tienda.Data;

namespace tienda.Controllers;

[Authorize(Policy = "RequiredAdminOrStaff")]
public class DashboardController : BaseController
{
    public DashboardController(OnlineShopDbContext context) 
        : base(context){ }

    public IActionResult Index()
    {
        return View();
    }
}
