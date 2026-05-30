using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnDemandHomeService.Models;

namespace OnDemandHomeService.Controllers
{
    public class AdminBookingController : Controller
    {
        private readonly AppDbContext _context;

        public AdminBookingController(AppDbContext context)
        {
            _context = context;
        }

        // READ - All bookings
        public IActionResult AdminBookingShow()
        {
            var data = _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Provider)
                .Include(b => b.Address)
                .Include(b => b.TimeSlot)
                .Include(b => b.Status)
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Service)
                .ToList();

            return View(data);
        }

        // DETAILS
        public IActionResult AdminBookingDetails(int id)
        {
            var booking = _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Provider)
                .Include(b => b.Address)
                .Include(b => b.TimeSlot)
                .Include(b => b.Status)
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Service)
                .FirstOrDefault(b => b.BookingId == id);

            return View(booking);
        }

        // CREATE
        public IActionResult AdminBookingCreate()
        {
            ViewBag.Users = _context.Users.ToList();
            ViewBag.Statuses = _context.BookingStatuses.ToList();
            ViewBag.TimeSlots = _context.TimeSlots.ToList();
            ViewBag.Addresses = _context.Addresses.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult AdminBookingCreate(Booking booking)
        {
            booking.CreatedAt = DateTime.Now;
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return RedirectToAction("AdminBookingShow");
        }

        // EDIT
        public IActionResult AdminBookingEdit(int id)
        {
            var booking = _context.Bookings.Find(id);

            ViewBag.Statuses = _context.BookingStatuses.ToList();
            ViewBag.TimeSlots = _context.TimeSlots.ToList();

            return View(booking);
        }

        [HttpPost]
        public IActionResult AdminBookingEdit(Booking booking)
        {
            _context.Bookings.Update(booking);
            _context.SaveChanges();

            return RedirectToAction("AdminBookingShow");
        }

        // DELETE
        public IActionResult AdminBookingDelete(int id)
        {
            var booking = _context.Bookings.Find(id);
            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var booking = _context.Bookings.Find(id);
            _context.Bookings.Remove(booking);
            _context.SaveChanges();

            return RedirectToAction("AdminBookingShow");
        }
    }

}
