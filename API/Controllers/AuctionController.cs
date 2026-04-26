using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AuctionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
