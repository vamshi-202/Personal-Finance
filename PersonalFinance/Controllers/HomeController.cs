using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Data.Repository;
using PersonalFinance.Models.ViewModels;
using PersonalFinance.Models.ViewModels;

namespace PersonalFinance.Controllers
{
  public class HomeController : Controller
  {
    private readonly ITransactionRepository _transactionRepo;

    public HomeController(ITransactionRepository transactionRepo)
    {
      _transactionRepo = transactionRepo;
    }

    public async Task<IActionResult> Index()
    {
      var userId = HttpContext.Session.GetInt32("UserId");
      if (userId == null)
        return RedirectToAction("Login", "User");

      var (income, expense) = await _transactionRepo.GetDashboardSummaryAsync(userId.Value);

      var model = new DashboardViewModel
      {
        TotalIncome = income,
        TotalExpense = expense
      };

      return View(model);
    }
  }
}
