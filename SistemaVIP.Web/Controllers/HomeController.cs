using Microsoft.AspNetCore.Mvc;

namespace SistemaVIP.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
