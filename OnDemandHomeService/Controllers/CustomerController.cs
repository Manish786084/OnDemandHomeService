using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnDemandHomeService.Models;
using OnDemandHomeService.ViewModels;

namespace OnDemandHomeService.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDbContext _context;
        public CustomerController(AppDbContext context)
        {
                _context = context;
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult GiveReview(int bookingId, int providerId)
        {
            ReviewsVM vm = new ReviewsVM()
            {
                BookingId = bookingId,
                ProviderId = providerId
            };

            return View(vm);
        }
        [HttpPost]
        public IActionResult GiveReview(ReviewsVM vm)
        {
            if (ModelState.IsValid)
            {
                int? customerId = HttpContext.Session.GetInt32("UserId");

                if (customerId == null)
                {
                    return RedirectToAction("RegisterLogin", "Auth");
                }

                // Prevent duplicate review
                bool alreadyReviewed = _context.Reviews
                    .Any(x => x.BookingId == vm.BookingId
                           && x.CustomerId == customerId);

                if (alreadyReviewed)
                {
                    TempData["error"] = "You already reviewed this booking.";
                    return RedirectToAction("MyBookings","Booking");
                }

                Review review = new Review()
                {
                    BookingId = vm.BookingId,
                    CustomerId = customerId.Value,
                    ProviderId = vm.ProviderId,
                    Rating = vm.Rating,
                    Comment = vm.Comment,
                    CreatedAt = DateTime.Now
                };

                _context.Reviews.Add(review);
                _context.SaveChanges();

                TempData["success"] = "Review submitted successfully.";

                return RedirectToAction("MyBookings","Booking");
            }

            return View(vm);
        }
    }
}
