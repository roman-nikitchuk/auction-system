using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BidController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
