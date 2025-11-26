using Microsoft.AspNetCore.Mvc;

namespace PersonalFinance.Controllers
{
  public class PublicController : Controller
  {
    // Landing page shown to all visitors
    public IActionResult Index()
    {
      return View();
    }
  }
}
