using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnDemandHomeService.Filters;

using OnDemandHomeService.Models;
using OnDemandHomeService.ViewModels;

using System.Linq;

namespace OnDemandHomeService.Controllers
{
    public class ProviderController : Controller
    {
        private readonly AppDbContext _context;
        public ProviderController(AppDbContext context)
        {
            _context = context;
        }
        [RoleAuthorize(2)]
        public IActionResult Dashboard()
        {
            var providerId = HttpContext.Session.GetInt32("UserId");

            if (providerId == null)
                return RedirectToAction("RegisterLogin", "Auth");

            // TODAY EARNINGS
            var todayEarnings = _context.Payments
                .Include(p => p.Booking)
                .Where(p =>
                    p.Booking.ProviderId == providerId &&
                    p.PaymentStatus == "Paid" &&
                    p.PaidAt.Value.Date == DateTime.Today)
                .Sum(p => (decimal?)p.Amount) ?? 0;

            // COMPLETED JOBS
            var completedJobs = _context.Bookings
            .Count(b =>
             b.ProviderId == providerId &&
             b.Status.StatusName == "Completed");

            // NEW REQUESTS
            var newRequests = _context.Bookings
                .Count(b =>
                    b.ProviderId == providerId &&
                    b.Status.StatusName == "Pending");

            // AVERAGE RATING
            var avgRating = _context.Reviews
                .Where(r => r.Booking.ProviderId == providerId)
                .Average(r => (double?)r.Rating) ?? 0;

            // PENDING BOOKINGS
            var pendingBookings = _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Address)
                .Include(b => b.TimeSlot)
                .Include(b => b.Status)
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Service)

                  .Where(b =>
                  b.ProviderId == providerId &&
                  b.Status.StatusName != "Paid" &&
                  b.Status.StatusName != "Cancelled")

                .Select(b => new PendingBookingVM
                {
                    BookingId = b.BookingId,

                    ServiceName = b.BookingDetails
                        .Select(x => x.Service.ServiceName)
                        .FirstOrDefault(),

                    CustomerName = b.Customer.FullName,

                    Address = b.Address.Street + ", " + b.Address.City,

                    Time = b.BookingDate + " | " +
                           b.TimeSlot.StartTime + " - " +
                           b.TimeSlot.EndTime,

                    Amount = b.TotalAmount
                })
                .ToList();

            // TODAY SCHEDULE
            var schedules = _context.Bookings
                .Include(b => b.Address)
                .Include(b => b.TimeSlot)
                .Include(b => b.Status)
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Service)

                .Where(b =>
                    b.ProviderId == providerId &&
                    b.BookingDate == DateOnly.FromDateTime(DateTime.Today) &&
                    b.Status.StatusName != "Cancelled")

                .Select(b => new TodayScheduleVM
                {
                    ServiceName = b.BookingDetails
                        .Select(x => x.Service.ServiceName)
                        .FirstOrDefault(),

                    Address = b.Address.Street,

                    Time = b.TimeSlot.StartTime + " - " +
                           b.TimeSlot.EndTime,

                    Amount = b.TotalAmount,

                    Status = b.Status.StatusName
                })
                .ToList();

            //REVIEWS
            //var reviews = _context.Reviews
            //    .Include(r => r.Customer)
            //    .Where(r => r.Booking.ProviderId == providerId)

            //    .OrderByDescending(r => r.CreatedAt)

            //    .Take(5)

            //    .Select(r => new ReviewsVM
            //    {
            //        CustomerName = r.Customer.FullName,
            //        Rating = r.Rating,
            //        Comment = r.Comment,
            //        CreatedAt = r.CreatedAt
            //    })
            //    .ToList();
            var reviews = _context.Reviews
     .Include(r => r.Customer)
     .Where(r => r.ProviderId == providerId)
     .OrderByDescending(r => r.CreatedAt)
     .Take(5)
     .Select(r => new ReviewsVM
     {
         BookingId = r.BookingId,
         ProviderId = r.ProviderId,
         CustomerName = r.Customer.FullName,
         Rating = r.Rating,
         Comment = r.Comment,
         CreatedAt = r.CreatedAt
     })
     .ToList();

            // WEEKLY EARNINGS
            var weekly = new List<decimal>();

            for (int i = 6; i >= 0; i--)
            {
                var day = DateTime.Today.AddDays(-i);

                var amount = _context.Payments
                    .Include(p => p.Booking)
                    .Where(p =>
                        p.Booking.ProviderId == providerId &&
                        p.PaymentStatus == "Paid" &&
                        p.PaidAt.Value.Date == day.Date)
                    .Sum(p => (decimal?)p.Amount) ?? 0;

                weekly.Add(amount);
            }

            var vm = new ProviderDashboardVM
            {
                TodayEarnings = todayEarnings,
                CompletedJobs = completedJobs,
                AvgRating = Math.Round(avgRating, 1),
                NewRequests = newRequests,
                PendingBookings = pendingBookings,
                TodaySchedules = schedules,
                Reviews = reviews,
                WeeklyEarnings = weekly
            };

            return View(vm);
        }
        // SHOW PROVIDERS & EDIT FORM

        public IActionResult ShowProvider(int? id)
        {
            var vm = new ProviderVM
            {
                Providers = _context.Users.Where(u => u.RoleId == 2).ToList(),
                NewProvider = new User(),
                IsEdit = false
            };

            if (id != null)
            {
                var provider = _context.Users.FirstOrDefault(u => u.UserId == id && u.RoleId == 2);
                if (provider != null)
                {
                    vm.NewProvider = provider;
                    vm.IsEdit = true;
                }
            }

            return View(vm);
        }

        // SAVE EDITED PROVIDER
        [HttpPost]
        public IActionResult SaveProvider(ProviderVM vm)
        {
            if (vm.NewProvider.UserId != 0)
            {
                var existing = _context.Users.Find(vm.NewProvider.UserId);
                if (existing != null)
                {
                    // Check if email already exists for another user
                    bool emailExists = _context.Users
                        .Any(u => u.Email == vm.NewProvider.Email && u.UserId != vm.NewProvider.UserId);
                    bool PhoneExists = _context.Users
                        .Any(u => u.Phone == vm.NewProvider.Phone && u.UserId != vm.NewProvider.UserId);

                    if (emailExists)
                    {
                        TempData["Error"] = "Email already exists for another provider!";
                        return RedirectToAction("ShowProvider", new { id = vm.NewProvider.UserId });
                    }
                    if (PhoneExists)
                    {
                        TempData["Error"] = "Phone Number already exists for another provider!";
                        return RedirectToAction("ShowProvider", new { id = vm.NewProvider.UserId });
                    }

                    existing.FullName = vm.NewProvider.FullName;
                    existing.Email = vm.NewProvider.Email;
                    existing.Phone = vm.NewProvider.Phone;
                    existing.IsActive = vm.NewProvider.IsActive;

                    _context.SaveChanges();
                }
            }

            return RedirectToAction("ShowProvider");
        }


        // TOGGLE ACTIVE / INACTIVE
        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {
            var provider = _context.Users.FirstOrDefault(u => u.UserId == id && u.RoleId == 2);
            if (provider != null)
            {
                provider.IsActive = !provider.IsActive;
                _context.SaveChanges();
            }
            return RedirectToAction("ShowProvider");
        }

        public IActionResult Bookings()
        {
            var data = _context.Bookings
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Service)
                .Include(b => b.Customer)
                .Include(b => b.Address)
                .Include(b => b.TimeSlot)
                .Include(b => b.Status)
                .ToList();

            var vm = data.Select(b => new ProviderBookingVM
            {
                BookingId = b.BookingId,

                CustomerName = b.Customer.FullName,
                PhoneNumber = b.Customer.Phone,

                ServiceNames = string.Join(", ",
                    b.BookingDetails.Select(d => d.Service.ServiceName)),

                BookingDate = b.BookingDate
                    .ToDateTime(TimeOnly.MinValue)
                    .ToString("dd-MM-yyyy"),

                TimeSlot = b.TimeSlot.StartTime + " - " + b.TimeSlot.EndTime,

                Address =
                    b.Address.Street + ", " +
                    b.Address.City + ", " +
                    b.Address.State + " - " +
                    b.Address.Pincode,

                Status = b.Status.StatusName,

                TotalAmount = b.TotalAmount
            }).ToList();

            return View(vm);
        }
        public IActionResult ShowAddService()
        {
            var services = _context.Services
                .Include(s => s.Category)
                .ToList();

            return View(services);
        }

        
        public IActionResult CreateService(int? id)
        {
            ViewBag.Categories = new SelectList(
                _context.Categories,
                "CategoryId",
                "CategoryName"
            );

            // EDIT MODE
            if (id != null)
            {
                var service = _context.Services.Find(id);

                if (service == null)
                {
                    return NotFound();
                }

                return View(service);
            }

            // CREATE MODE
            return View(new Service());
        }


        // =============================
     

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateService(Service service)
        {
            if (ModelState.IsValid)
            {
                bool exists = _context.Services.Any(x =>
                    x.ServiceName.ToLower() == service.ServiceName.ToLower()
                    && x.ServiceId != service.ServiceId);

                if (exists)
                {
                    ModelState.AddModelError("ServiceName",
                        "Service already exists.");

                    ViewBag.Categories = new SelectList(
                        _context.Categories,
                        "CategoryId",
                        "CategoryName"
                    );

                    return View(service);
                }

                // ADD
                if (service.ServiceId == 0)
                {
                    service.IsActive = true;

                    _context.Services.Add(service);

                    TempData["Success"] =
                        "Service added successfully.";
                }
                // UPDATE
                else
                {
                    var existing = _context.Services
                        .Find(service.ServiceId);

                    if (existing == null)
                    {
                        return NotFound();
                    }

                    existing.ServiceName = service.ServiceName;
                    existing.CategoryId = service.CategoryId;
                    existing.Description = service.Description;
                    existing.BasePrice = service.BasePrice;
                    existing.DurationMinutes = service.DurationMinutes;
                    existing.IsActive = service.IsActive;

                    TempData["Success"] =
                        "Service updated successfully.";
                }

                _context.SaveChanges();

                return RedirectToAction("ShowAddService");
            }

            ViewBag.Categories = new SelectList(
                _context.Categories,
                "CategoryId",
                "CategoryName"
            );

            return View(service);
        }
        // =============================
        // DELETE SERVICE
        // =============================
        public IActionResult DeleteService(int id)
        {
            var service = _context.Services.Find(id);

            if (service == null)
            {
                TempData["Error"] = "Service not found.";
                return RedirectToAction("ShowAddService");
            }

            // Check relation with ProviderServices
            bool isLinked = _context.ProviderServices
                                    .Any(x => x.ServiceId == id);

            if (isLinked)
            {
                // Soft Delete
                service.IsActive = false;

                _context.Services.Update(service);
                _context.SaveChanges();

                TempData["Success"] = "Service deactivated because it is linked with providers/bookings.";
            }
            else
            {
                // Hard Delete
                _context.Services.Remove(service);
                _context.SaveChanges();

                TempData["Success"] = "Service deleted successfully.";
            }

            return RedirectToAction("ShowAddService");
        }
        public IActionResult NewRequest()
        {
            var providerId = HttpContext.Session.GetInt32("UserId");

            if (providerId == null)
                return RedirectToAction("RegisterLogin", "Auth");
            var pendingBookings = _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Address)
                .Include(b => b.TimeSlot)
                .Include(b => b.Status)
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Service)

                .Where(b =>
                b.ProviderId == providerId &&
                b.Status.StatusName != "Paid" &&
                b.Status.StatusName != "Cancelled")

                .Select(b => new PendingBookingVM
                {
                    BookingId = b.BookingId,

                    ServiceName = b.BookingDetails
                        .Select(x => x.Service.ServiceName)
                        .FirstOrDefault(),

                    CustomerName = b.Customer.FullName,

                    Address = b.Address.Street + ", " + b.Address.City,

                    Time = b.BookingDate + " | " +
                           b.TimeSlot.StartTime + " - " +
                           b.TimeSlot.EndTime,

                    Amount = b.TotalAmount
                })
                .ToList();
            var vm = new ProviderDashboardVM
            {
                PendingBookings = pendingBookings
            };

            // ✅ Pass model to view
            return View(vm);
        }
        public IActionResult Earnings()
        {
            var providerId = HttpContext.Session.GetInt32("UserId");

            if (providerId == null)
                return RedirectToAction("RegisterLogin", "Auth");

            // ALL PAYMENTS
            var payments = _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.BookingDetails)
                        .ThenInclude(d => d.Service)

                .Where(p =>
                    p.Booking.ProviderId == providerId &&
                    p.PaymentStatus == "Paid")

                .OrderByDescending(p => p.PaidAt)
                .ToList();

            // TODAY
            ViewBag.TodayEarning = payments
                .Where(p => p.PaidAt?.Date == DateTime.Today)
                .Sum(p => p.Amount);

            // MONTH
            ViewBag.MonthEarning = payments
                .Where(p =>
                    p.PaidAt?.Month == DateTime.Now.Month &&
                    p.PaidAt?.Year == DateTime.Now.Year)
                .Sum(p => p.Amount);

            // TOTAL
            ViewBag.TotalEarning = payments
                .Sum(p => p.Amount);

            return View(payments);
        }
    }
}
