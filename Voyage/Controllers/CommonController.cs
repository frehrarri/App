using Microsoft.AspNetCore.Mvc;

namespace Voyage.Controllers
{
    public class CommonController : Controller
    {
        [HttpGet]
        public IActionResult GridControlPartial()
        {
            return PartialView("~/Views/Shared/_GridControl.cshtml");
        }
    }
}
