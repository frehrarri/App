using Microsoft.AspNetCore.Mvc;

namespace Voyage.Controllers
{
    public class MarketingController : Controller
    {
        public IActionResult MarketingPartial()
        {
            return View("~/Views/App/Marketing/_Marketing.cshtml");
        }
    }
}
