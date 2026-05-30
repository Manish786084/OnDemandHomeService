using Microsoft.AspNetCore.Mvc;
using OnDemandHomeService.Helpers;
using OnDemandHomeService.Models;
using OnDemandHomeService.ViewModels;

namespace OnDemandHomeService.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // Single page for login & register
        public IActionResult RegisterLogin()
        {
            return View();
        }

        // Ajax registration
        [HttpPost]
        public IActionResult Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Please fill all fields correctly." });

            // Check if email exists
            if (_context.Users.Any(u => u.Email == model.Email))
                return Json(new { success = false, message = "Email already registered." });
            if (_context.Users.Any(u => u.Phone == model.Phone))
                return Json(new { success = false, message = "Phone already registered." });

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                PasswordHash = PasswordHelper.HashPassword(model.Password),
                RoleId = model.RoleId,
                IsActive = true
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Json(new { success = true });
        }

        // Ajax login
        [HttpPost]
        public IActionResult Login(LoginVM model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Please fill all fields correctly." });

            var hash = PasswordHelper.HashPassword(model.Password);
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.PasswordHash == hash || u.PasswordHash == model.Password);

            if (user == null)
                return Json(new { success = false, message = "Invalid credentials." });

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetInt32("Role", user.RoleId);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("FullName", user.FullName);

            // Role-based redirect
            string redirect = user.RoleId switch
            {
                1 => Url.Action("Index", "Home"),
                2 => Url.Action("Dashboard", "Provider"),
                3 => Url.Action("Dashboard", "Admin"),
                _ => Url.Action("Index", "Home")
            };

            return Json(new { success = true, redirect });
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("RegisterLogin");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
