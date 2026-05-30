using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnDemandHomeService.Models;
using OnDemandHomeService.Services;
using OnDemandHomeService.ViewModels;

namespace OnDemandHomeService.Controllers
{
    public class ProviderServiceController : Controller
    {
        private readonly AppDbContext _context;
        private readonly NotificationService _notification;

        public ProviderServiceController(AppDbContext context, NotificationService notification)
        {
            _context = context;
            _notification = notification;
        }

        // LIST + EDIT FORM
        public IActionResult MyServices(int? id)
        {
            var providerId = HttpContext.Session.GetInt32("UserId");
            if (providerId == null)
                return RedirectToAction("RegisterLogin", "Auth");

            var services = _context.ProviderServices
                .Include(ps => ps.Service)
                .Where(ps => ps.ProviderId == providerId)
                .ToList();

            ViewBag.AllServices = new SelectList(
                _context.Services.ToList(),
                "ServiceId",
                "ServiceName"
            );

            if (id != null)
            {
                var edit = _context.ProviderServices
                    .FirstOrDefault(x => x.ProviderServiceId == id && x.ProviderId == providerId);

                ViewBag.EditService = edit;
            }

            return View(services);
        }

        // ADD / UPDATE
        [HttpPost]
        public IActionResult SaveService(ProviderService psModel)
        {
            var providerId = HttpContext.Session.GetInt32("UserId");
            if (providerId == null)
                return RedirectToAction("RegisterLogin", "Auth");

            if (psModel.ProviderServiceId == 0)
            {
                // New service
                var exists = _context.ProviderServices
                    .Any(x => x.ProviderId == providerId && x.ServiceId == psModel.ServiceId);

                if (exists)
                {
                    TempData["Error"] = "Service already added";
                    return RedirectToAction("MyServices");
                }

                psModel.ProviderId = providerId.Value;
                psModel.IsApproved = false; // always pending

                _context.ProviderServices.Add(psModel);
            }
            else
            {
                // Edit existing
                var existing = _context.ProviderServices
                    .FirstOrDefault(x => x.ProviderServiceId == psModel.ProviderServiceId
                                         && x.ProviderId == providerId);

                if (existing != null)
                {
                    existing.ServiceId = psModel.ServiceId;
                    existing.PriceOverride = psModel.PriceOverride;
                    existing.ExperienceYears = psModel.ExperienceYears;

                    // DO NOT change approval here
                }
            }

            _context.SaveChanges();
            TempData["Success"] = "Service saved successfully";

            return RedirectToAction("MyServices");
        }

        // DELETE
        [HttpPost]
        public IActionResult DeleteService(int id)
        {
            var providerId = HttpContext.Session.GetInt32("UserId");
            if (providerId == null)
                return RedirectToAction("RegisterLogin", "Auth");

            var ps = _context.ProviderServices
                .FirstOrDefault(x => x.ProviderServiceId == id && x.ProviderId == providerId);

            if (ps != null)
            {
                _context.ProviderServices.Remove(ps);
                _context.SaveChanges();
                TempData["Success"] = "Service deleted successfully";
            }

            return RedirectToAction("MyServices");
        }
        public IActionResult Bookings()
        {
            var providerId = HttpContext.Session.GetInt32("UserId");
            if (providerId == null)
                return RedirectToAction("RegisterLogin", "Auth");

            var data = _context.Bookings
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Service)
                .Include(b => b.Customer)
                .Include(b => b.Address)
                .Include(b => b.TimeSlot)
                .Include(b => b.Status)
                .Where(b => b.ProviderId == providerId)

                .OrderByDescending(b => b.BookingId)
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
        [HttpPost]
        public IActionResult Accept(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking != null)
            {
                booking.StatusId = 2; // accepted
                _context.SaveChanges();
             _notification.Send(
            booking.CustomerId,
            "Booking Accepted",
            "Provider accepted your booking");
            }
            return RedirectToAction("ManageBookingRequest", new { id = id });
        }

        [HttpPost]
        public IActionResult Cancel(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking != null)
            {
                booking.StatusId = 5; // Cancelled
                _context.SaveChanges();
                _notification.Send(
                booking.ProviderId.Value,
               "Booking Cancelled",
               "Customer has cancelled the booking");
            }
            return RedirectToAction("ManageBookingRequest", new { id = id });
        }

        [HttpPost]
        public IActionResult Complete(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking != null)
            {
                booking.StatusId = 4; // Completed
                _context.SaveChanges();

                _notification.Send(
            booking.CustomerId,
            "Service Completed",
            "Your service is completed successfully");
            }
            return RedirectToAction("ManageBookingRequest", new { id = id });
        }
        public IActionResult Pay(int id)
        {
            var booking = _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Service)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null || booking.StatusId != 4)
                return Content("Invalid booking");

            var vm = new PaymentVM
            {
                BookingId = booking.BookingId,
                CustomerName = booking.Customer.FullName,
                ServiceName = string.Join(", ",
                                booking.BookingDetails
                                .Select(d => d.Service.ServiceName)),
                Amount = booking.TotalAmount ?? 0   // safer
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Pay(PaymentVM model)
        {
            var booking = _context.Bookings
                .FirstOrDefault(b => b.BookingId == model.BookingId);

            if (booking == null || booking.StatusId != 4)
                return Content("Invalid booking");

            // 🔒 Always take amount from database (NOT from model)
            var payment = new Payment
            {
                BookingId = booking.BookingId,
                Amount = booking.TotalAmount ?? 0,
                PaymentMethod = model.PaymentMethod,
                PaymentStatus = "Paid",
                PaidAt = DateTime.Now
            };

            _context.Payments.Add(payment);
            _context.SaveChanges(); // Save first to generate PaymentId

            // Create Transaction
            var transaction = new Transaction
            {
                PaymentId = payment.PaymentId,
                TransactionRef = "TXN" + Guid.NewGuid().ToString("N").Substring(0, 8),
                Status = "Success",
                CreatedAt = DateTime.Now
            };

            _context.Transactions.Add(transaction);

            // ✅ IMPORTANT: Change booking status AFTER payment
            booking.StatusId = 8; // Add new status: Paid or Closed

            _context.SaveChanges();
            _notification.Send(
            booking.ProviderId.Value,
            "Payment Received",
            "Customer has completed payment"
);

            TempData["success"] = "Payment successful!";

            return RedirectToAction("MyBookings","Booking");
        }


        public IActionResult ManageBookingRequest(int id)
        {
            var providerId = HttpContext.Session.GetInt32("UserId");

            if (providerId == null)
                return RedirectToAction("RegisterLogin", "Auth");

            var booking = _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Address)
                .Include(b => b.TimeSlot)
                .Include(b => b.Status)
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Service)
                .FirstOrDefault(b =>
                    b.BookingId == id &&
                    b.ProviderId == providerId);

            if (booking == null)
                return NotFound();

            return View(booking);
        }
    }
}
