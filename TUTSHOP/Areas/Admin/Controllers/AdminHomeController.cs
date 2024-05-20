using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TUTSHOP.Models.Entities;

namespace TUTSHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AdminHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
