using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnDemandHomeService.Models;

namespace OnDemandHomeService.Controllers
{
    public class CustomerServiceController : Controller
    {
        private readonly AppDbContext _context;

        public CustomerServiceController(AppDbContext context)
        {
            _context = context;
        }

        // Show all services as cards
     
        public IActionResult UserServiceList()
        {
            var services = _context.Services
                .Include(s => s.Category)
                .Where(s => s.IsActive)
                .OrderBy(s => s.ServiceName)
                .ToList();

            return View(services);
        }

    }
}
