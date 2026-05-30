using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnDemandHomeService.Models;
using OnDemandHomeService.Services;
using OnDemandHomeService.ViewModels;
//using QuestPDF.Fluent;
//using QuestPDF.Helpers;
//using QuestPDF.Infrastructure;
//using QuestPDF.Helpers;

namespace OnDemandHomeService.Controllers
{
    public class BookingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly NotificationService _notification;

        public BookingController(AppDbContext context, NotificationService notification)
        {
            _context = context;
            _notification = notification;
        }

        // Show booking form
        public IActionResult Create(int serviceId)
        {
            var service = _context.Services
                .Include(s => s.Category)
                .FirstOrDefault(s => s.ServiceId == serviceId);

            if (service == null)
                return Content("Service not found");

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("RegisterLogin", "Auth");

            ViewBag.Addresses = _context.Addresses
                .Where(a => a.UserId == userId)
                .ToList();

            ViewBag.TimeSlots = _context.TimeSlots
                .Where(t => t.IsActive==true)
                .ToList();

            return View(service);
        }


        //[HttpPost]
        //public IActionResult Create(int serviceId, int addressId, DateTime bookingDate, int timeSlotId)
        //{
        //    var customerId = HttpContext.Session.GetInt32("UserId");

        //    if (customerId == null)
        //        return RedirectToAction("RegisterLogin", "Auth");

        //    var service = _context.Services
        //        .Include(s => s.ProviderServices)
        //        .FirstOrDefault(s => s.ServiceId == serviceId);

        //    if (service == null)
        //        return Content("Service not found");

        //    // GET PROVIDER
        //    var providerId = service.ProviderServices
        //        .FirstOrDefault()?.ProviderId;
        //   // var service = _context.Services
        //   //.FirstOrDefault(s => s.ServiceId == serviceId);

        //    if (service == null)
        //        return Content("Service not found");

        //    // DEBUG


        //    var booking = new Booking
        //    {
        //        CustomerId = customerId.Value,
        //        ProviderId = null,
        //        //ProviderId = providerId,
        //        AddressId = addressId,
        //        BookingDate = DateOnly.FromDateTime(bookingDate),
        //        TimeSlotId = timeSlotId,
        //        StatusId = 1,
        //        CreatedAt = DateTime.Now,
        //        TotalAmount = service.BasePrice
        //    };

        //    _context.Bookings.Add(booking);
        //    _context.SaveChanges();

        //    var detail = new BookingDetail
        //    {
        //        BookingId = booking.BookingId,
        //        ServiceId = serviceId,
        //        Price = service.BasePrice,
        //        Quantity = 1
        //    };

        //    _context.BookingDetails.Add(detail);
        //    _context.SaveChanges();

        //    return RedirectToAction("MyBookings");
        //}
        //[HttpPost]
        //public IActionResult Create(int serviceId, int addressId, DateTime bookingDate, int timeSlotId)
        //{
        //    var customerId = HttpContext.Session.GetInt32("UserId");

        //    if (customerId == null)
        //        return RedirectToAction("RegisterLogin", "Auth");

        //    var service = _context.Services
        //        .Include(s => s.ProviderServices)
        //        .FirstOrDefault(s => s.ServiceId == serviceId);

        //    if (service == null)
        //        return Content("Service not found");

        //    // GET PROVIDER
        //    var providerId = service.ProviderServices
        //        .FirstOrDefault()?.ProviderId;

        //    if (providerId == null)
        //    {
        //        TempData["Error"] = "No provider available for this service.";
        //        return RedirectToAction("UserServiceList", "CustomerService");
        //    }

        //    var booking = new Booking
        //    {
        //        CustomerId = customerId.Value,
        //        ProviderId = providerId,
        //        AddressId = addressId,
        //        BookingDate = DateOnly.FromDateTime(bookingDate),
        //        TimeSlotId = timeSlotId,
        //        StatusId = 1,
        //        CreatedAt = DateTime.Now,
        //        TotalAmount = service.BasePrice
        //    };

        //    _context.Bookings.Add(booking);
        //    _context.SaveChanges();



        //    var detail = new BookingDetail
        //    {
        //        BookingId = booking.BookingId,
        //        ServiceId = serviceId,
        //        Price = service.BasePrice,
        //        Quantity = 1
        //    };

        //    _context.BookingDetails.Add(detail);
        //    _context.SaveChanges();

        //    return RedirectToAction("MyBookings");
        //}
        [HttpPost]
        public IActionResult Create(int serviceId, int addressId, DateTime bookingDate, int timeSlotId)
        {
            var customerId = HttpContext.Session.GetInt32("UserId");

            if (customerId == null)
                return RedirectToAction("RegisterLogin", "Auth");

            var service = _context.Services
                .Include(s => s.ProviderServices)
                .FirstOrDefault(s => s.ServiceId == serviceId);

            if (service == null)
                return Content("Service not found");

            var providerId = service.ProviderServices
                .Select(p => p.ProviderId)
                .FirstOrDefault();

            if (providerId == 0)
            {
                TempData["Error"] = "No provider available for this service.";
                return RedirectToAction("UserServiceList", "CustomerService");
            }

            var booking = new Booking
            {
                CustomerId = customerId.Value,
                ProviderId = providerId,
                AddressId = addressId,
                BookingDate = DateOnly.FromDateTime(bookingDate),
                TimeSlotId = timeSlotId,
                StatusId = 1,
                CreatedAt = DateTime.Now,
                TotalAmount = service.BasePrice
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            // 👉 BOOKING DETAIL
            _context.BookingDetails.Add(new BookingDetail
            {
                BookingId = booking.BookingId,
                ServiceId = serviceId,
                Price = service.BasePrice,
                Quantity = 1
            });

            _context.SaveChanges();

            // 🔔 NOTIFICATION 1 → Provider
            //_context.Notifications.Add(new Notification
            //{
            //    UserId = providerId,
            //    Title = "New Booking",
            //    Message = "You have received a new booking request",
            //    IsRead = false,
            //    CreatedAt = DateTime.Now
            //});

            //// 🔔 NOTIFICATION 2 → Customer
            //_context.Notifications.Add(new Notification
            //{
            //    UserId = customerId.Value,
            //    Title = "Booking Created",
            //    Message = "Your booking has been placed successfully",
            //    IsRead = false,
            //    CreatedAt = DateTime.Now
            //});
            AddNotification(providerId, "New Booking", "You have received a new booking");
            AddNotification(customerId.Value, "Booking Confirmed", "Your booking has been placed");
          
            _context.SaveChanges();

            return RedirectToAction("MyBookings");
        }

        // Customer booking list
        public IActionResult MyBookings()
        {
            var customerId = HttpContext.Session.GetInt32("UserId");
            if (customerId == null) return RedirectToAction("RegisterLogin", "Auth");

            
            var bookings = _context.Bookings
     .Include(x => x.BookingDetails)
         .ThenInclude(x => x.Service)
     .Include(x => x.TimeSlot)
     .Include(x => x.Status)
     .Include(x => x.Reviews)
     .Where(x => x.CustomerId == customerId)
     .OrderByDescending(x => x.BookingId)
     .ToList();


            return View(bookings);
        }
        public IActionResult Cancel(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == id);

            if (booking != null)
            {
                booking.StatusId = 5; // Cancelled
                _context.SaveChanges();

                // Get provider id from booking
                var providerId = booking.ProviderId;
                var customerId = booking.CustomerId;

                // Notify provider
                AddNotification(
                    providerId.Value,
                    "Booking Cancelled",
                    "Customer has cancelled the booking"
                );
                AddNotification(
                    customerId,
                    "Booking Cancelled",
                    "You have cancelled your service!"
                );
            }

            return RedirectToAction("MyBookings");
        }
        public IActionResult AddAddress(int serviceId)
        {
            ViewBag.ServiceId = serviceId;
            return View();
        }


        [HttpPost]
        public IActionResult AddAddress(Address address, int serviceId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("RegisterLogin", "Auth");

            address.UserId = userId.Value;

            _context.Addresses.Add(address);
            _context.SaveChanges();

            return RedirectToAction("Create", new { serviceId = serviceId });
        }

        public void AddNotification(int userId, string title, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }
    }
}
