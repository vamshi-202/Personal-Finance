using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PersonalFinance.Data.Models;
using PersonalFinance.Data.Repository;
using PersonalFinance.Helpers;
using PersonalFinance.Models.DTO.User;

namespace PersonalFinance.Controllers
{
  public class UserController : Controller
  {
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
      _userRepository = userRepository;
    }

    private int? GetCurrentUserId() => HttpContext.Session.GetInt32("UserId");

    public IActionResult Index() => View();

    // ---------------- Register ----------------
    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
      if (!ModelState.IsValid)
      {
        TempData["Error"] = "Validation failed.";
        return View(dto);
      }

      var hashedPassword = PasswordHelper.HashPassword(dto.Password);
      var user = new User
      {
        UserName = dto.UserName,
        Email = dto.Email,
        Password = hashedPassword
      };

      var result = await _userRepository.CreateUserAsync(user);
      if (!result)
      {
        TempData["Error"] = "Registration failed. Try again.";
        return View(dto);
      }

      TempData["Success"] = "Registration successful. Please log in.";
      return RedirectToAction("Login");
    }

    // ---------------- Login ----------------
    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginUserDto dto)
    {
      if (!ModelState.IsValid)
        return View(dto);

      var user = await _userRepository.GetUserByUserNameAsync(dto.UserName);
      if (user == null || !PasswordHelper.VerifyPassword(dto.Password, user.Password))
      {
        TempData["Error"] = "Invalid username or password.";
        return View(dto);
      }

      HttpContext.Session.SetInt32("UserId", user.UserId);
      HttpContext.Session.SetString("UserName", user.UserName);

      return RedirectToAction("Index", "Home");
    }

    // ---------------- Logout ----------------
    public IActionResult Logout()
    {
      HttpContext.Session.Clear();
      TempData["Success"] = "Logged out successfully.";
      return RedirectToAction("Login");
    }

    // ---------------- Profile ----------------
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
      var userId = GetCurrentUserId();
      if (userId == null) return RedirectToAction("Login");

      var user = await _userRepository.GetUserByUserIdAsync(userId.Value);
      if (user == null) return RedirectToAction("Login");

      var dto = new UpdateUserDto
      {
        UserId = user.UserId,
        UserName = user.UserName,
        Email = user.Email
      };

      return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(UpdateUserDto dto)
    {
      if (!ModelState.IsValid)
        return View(dto);

      var user = new User
      {
        UserId = dto.UserId,
        UserName = dto.UserName,
        Email = dto.Email
      };

      var result = await _userRepository.UpdateUserAsync(user);
      TempData["Success"] = result ? "Profile updated." : "Update failed.";
      return View(dto);
    }

    // ---------------- Change Password ----------------
    [HttpGet]
    public IActionResult ChangePassword() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
      var userId = GetCurrentUserId();
      if (userId == null) return RedirectToAction("Login");

      if (!ModelState.IsValid)
        return View(dto);

      var user = await _userRepository.GetUserByUserIdAsync(userId.Value);
      if (user == null || !PasswordHelper.VerifyPassword(dto.CurrentPassword, user.Password))
      {
        TempData["Error"] = "Current password is incorrect.";
        return View(dto);
      }

      var newHashed = PasswordHelper.HashPassword(dto.NewPassword);
      var result = await _userRepository.ChangePasswordAsync(userId.Value, newHashed);

      TempData["Success"] = result ? "Password changed successfully." : "Password change failed.";
      return View();
    }
  }
}
