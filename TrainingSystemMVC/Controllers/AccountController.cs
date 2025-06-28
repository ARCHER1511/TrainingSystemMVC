using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using TrainingSystemMVC.Services;
using TrainingSystemMVC.ViewModel;

namespace TrainingSystemMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService authService;
        public AccountController(AuthService _authService)
        {
            authService = _authService;
        }
        public IActionResult Register() => View();
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel register) 
        {
            if(!ModelState.IsValid) return View(register);
            var result = await authService.RegisterAsync(register);
            TempData["Message"] = result;
            return RedirectToAction("Login");
        }
        public IActionResult Login() => View();
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login) 
        {
            if (!ModelState.IsValid) return View(login);

            var loginResult = await authService.LoginAsync(login);

            if (loginResult != null) 
            {
                HttpContext.Session.SetString("JWToken", loginResult.Token);
                HttpContext.Session.SetString("Email", loginResult.Email);
                return RedirectToAction("Index", "Course");
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(login);
        }
        [Authorize]
        public IActionResult ProtectedArea()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken")))
                return RedirectToAction("Login");
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
