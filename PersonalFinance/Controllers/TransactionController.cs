using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using PersonalFinance.Data.Models;
using PersonalFinance.Data.Repository;
using PersonalFinance.Models.DTO.Transaction;

namespace PersonalFinance.Controllers
{
  public class TransactionController : Controller
  {
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;

    public TransactionController(
        ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository)
    {
      _transactionRepository = transactionRepository;
      _categoryRepository = categoryRepository;
    }

    private int? GetCurrentUserId()
    {
      return HttpContext.Session.GetInt32("UserId");
    }

    private async Task LoadCategoriesToViewBag()
    {
      var categories = await _categoryRepository.GetAllCategoriesAsync();
      ViewBag.Categories = categories.Select(c => new SelectListItem
      {
        Value = c.CategoryId.ToString(),
        Text = $"{c.CategoryName} ({c.Type})"
      }).ToList();
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? type, DateTime? startDate, DateTime? endDate)
    {
      var userId = GetCurrentUserId();
      if (userId == null)
        return RedirectToAction("Login", "User");

      var transactions = await _transactionRepository.GetFilteredTransactionsAsync(userId.Value, type, startDate, endDate);
      ViewBag.Type = type;
      ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
      ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

      return View(transactions);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
      if (GetCurrentUserId() == null)
        return RedirectToAction("Login", "User");

      await LoadCategoriesToViewBag();
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTransactionDto dto)
    {
      var userId = GetCurrentUserId();
      if (userId == null)
        return RedirectToAction("Login", "User");

      if (!ModelState.IsValid)
      {
        await LoadCategoriesToViewBag();
        return View(dto);
      }

      var transaction = new Transaction
      {
        UserId = userId.Value,
        CategoryId = dto.CategoryId,
        Amount = dto.Amount,
        Description = dto.Description,
        TransactionDate = dto.TransactionDate
      };

      var result = await _transactionRepository.AddTransactionAsync(transaction);
      if (result)
      {
        TempData["Success"] = "Transaction created successfully.";
        return RedirectToAction("Index");
      }

      ModelState.AddModelError("", "Transaction creation failed.");
      await LoadCategoriesToViewBag();
      return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      var userId = GetCurrentUserId();
      if (userId == null)
        return RedirectToAction("Login", "User");

      var transaction = await _transactionRepository.GetTransactionByTransactionIdAsync(id);
      if (transaction == null || transaction.UserId != userId)
        return NotFound();

      var dto = new UpdateTransactionDto
      {
        TransactionId = transaction.TransactionId,
        CategoryId = transaction.CategoryId,
        Amount = transaction.Amount,
        Description = transaction.Description,
        TransactionDate = transaction.TransactionDate
      };

      await LoadCategoriesToViewBag();
      return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UpdateTransactionDto dto)
    {
      var userId = GetCurrentUserId();
      if (userId == null)
        return RedirectToAction("Login", "User");

      if (!ModelState.IsValid)
      {
        await LoadCategoriesToViewBag();
        return View(dto);
      }

      var transaction = await _transactionRepository.GetTransactionByTransactionIdAsync(dto.TransactionId);
      if (transaction == null || transaction.UserId != userId)
        return NotFound();

      transaction.CategoryId = dto.CategoryId;
      transaction.Amount = dto.Amount;
      transaction.Description = dto.Description;
      transaction.TransactionDate = dto.TransactionDate;

      var result = await _transactionRepository.UpdateTransactionAsync(transaction);
      if (result)
      {
        TempData["Success"] = "Transaction updated successfully.";
        return RedirectToAction("Index");
      }

      ModelState.AddModelError("", "Transaction update failed.");
      await LoadCategoriesToViewBag();
      return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
      var userId = GetCurrentUserId();
      if (userId == null)
        return RedirectToAction("Login", "User");

      var transaction = await _transactionRepository.GetTransactionByTransactionIdAsync(id);
      if (transaction == null || transaction.UserId != userId)
        return NotFound();

      return View(transaction);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
      var userId = GetCurrentUserId();
      if (userId == null)
        return RedirectToAction("Login", "User");

      var transaction = await _transactionRepository.GetTransactionByTransactionIdAsync(id);
      if (transaction == null || transaction.UserId != userId)
        return NotFound();

      return View(transaction);
    }

    [HttpPost]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var userId = GetCurrentUserId();
      if (userId == null)
        return RedirectToAction("Login", "User");

      var transaction = await _transactionRepository.GetTransactionByTransactionIdAsync(id);
      if (transaction == null || transaction.UserId != userId)
        return NotFound();

      var result = await _transactionRepository.DeleteTransactionAsync(id);
      if (result)
        TempData["Success"] = "Transaction deleted successfully.";
      else
        TempData["Error"] = "Failed to delete transaction.";

      return RedirectToAction("Index");
    }
  }
}
