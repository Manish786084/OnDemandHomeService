using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnDemandHomeService.Filters;
using OnDemandHomeService.Models;
using OnDemandHomeService.ViewModels;
using System.Linq;

namespace OnDemandHomeService.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }
        [RoleAuthorize(3)]
        public IActionResult Dashboard()
        {
            var vm = new AdminDashboardVM();

            // New Requests
            vm.NewRequests = _context.Bookings
                .Count(x => x.StatusId == 1);

            // Ongoing Jobs
            vm.OngoingJobs = _context.Bookings
                .Count(x => x.StatusId == 2);

            // Active Providers
            vm.ActiveProviders = _context.Users
                .Count(x => x.RoleId == 2);

            // Today Revenue
            vm.TodayRevenue = _context.Payments
                .Where(x => x.PaidAt.Value.Date == DateTime.Today)
                .Sum(x => (decimal?)x.Amount) ?? 0;

            // Recent Bookings
            vm.RecentBookings = _context.Bookings
                .Include(x => x.Customer)
                .Include(x => x.Provider)
                .Include(x => x.Status)
                .Include(x => x.BookingDetails)
                    .ThenInclude(bd => bd.Service)
                .OrderByDescending(x => x.BookingId)
                .Take(5)
                .ToList();

            // Top Providers
            vm.TopProviders = _context.Bookings
                .Where(x => x.ProviderId != null)
                .GroupBy(x => x.Provider.FullName)
                .Select(g => new TopProviderVM
                {
                    ProviderName = g.Key,
                    TotalJobs = g.Count(),
                    ServiceName = g.First().BookingDetails
                        .FirstOrDefault().Service.ServiceName
                })
                .OrderByDescending(x => x.TotalJobs)
                .Take(3)
                .ToList();

            // Service Demand Chart
            var serviceDemand = _context.BookingDetails
                .GroupBy(x => x.Service.ServiceName)
                .Select(g => new
                {
                    Service = g.Key,
                    Count = g.Count()
                })
                .ToList();

            vm.ServiceLabels = serviceDemand.Select(x => x.Service).ToList();
            vm.ServiceCounts = serviceDemand.Select(x => x.Count).ToList();

            // Monthly Bookings
            var monthly = _context.Bookings
                .GroupBy(x => x.CreatedAt.Value.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToList();

            vm.MonthlyLabels = monthly
                .Select(x => System.Globalization.CultureInfo
                .CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(x.Month))
                .ToList();

            vm.MonthlyBookings = monthly
                .Select(x => x.Count)
                .ToList();

            // Ratings
            //vm.AverageRating = _context.Reviews.Any()
                //? _context.Reviews.Average(x => x.Rating)
                //: 0;

            //vm.PositiveReviews = _context.Reviews
            //    .Count(x => x.Rating >= 4);

            //vm.NegativeReviews = _context.Reviews
            //    .Count(x => x.Rating < 4);

            return View(vm);
        }

        // GET: Display the list of customers
        // GET: Display the list
        public IActionResult CustomerDetails()
        {
            var customers = _context.Users
                                   .Where(u => u.RoleId == 1)
                                   .OrderByDescending(u => u.CreatedAt)
                                   .ToList();
            return View(customers);
        }

        
        public IActionResult ServiceApprovals()
        {
            var list = _context.ProviderServices
                .Include(ps => ps.Provider)
                .Include(ps => ps.Service)
                .ToList();

            return View(list);
        }

        // APPROVE / REVOKE toggle
        [HttpPost]
        public IActionResult ToggleApproval(int id)
        {
            var ps = _context.ProviderServices.Find(id);
            if (ps != null)
            {
                ps.IsApproved = ps.IsApproved == true ? false : true;
                _context.SaveChanges();
            }

            return RedirectToAction("ServiceApprovals");
        }

        // DELETE service (optional reject)
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var ps = _context.ProviderServices.Find(id);
            if (ps != null)
            {
                _context.ProviderServices.Remove(ps);
                _context.SaveChanges();
            }

            return RedirectToAction("ServiceApprovals");
        }
    }

}
