using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnDemandHomeService.Models;

namespace OnDemandHomeService.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;
        public ProfileController(AppDbContext context)
        {
                _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetProfilePartial()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Unauthorized();

            var user = _context.Users
                .Where(u => u.UserId == userId)
                .FirstOrDefault();

            return PartialView("_UserProfileCard", user);
        }
    }
}
