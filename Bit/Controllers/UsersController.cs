using Microsoft.AspNetCore.Mvc;

namespace Bit.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
