using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnDemandHomeService.Models;
using System.Diagnostics;

namespace OnDemandHomeService.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger,AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var services = _context.Services
                .Include(s => s.Category)   // ?? important
                .ToList();

            return View(services);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(Contact contact)
        {
            if (ModelState.IsValid)
            {
                contact.CreatedAt = DateTime.Now;

                _context.Contacts.Add(contact);
                _context.SaveChanges();
                TempData["name"] = contact.FullName;
                TempData["success"] = "Your message has been sent successfully.";

                return RedirectToAction("Contact");
            }

            return View(contact);
        }
    }
}
