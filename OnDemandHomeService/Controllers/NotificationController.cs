using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnDemandHomeService.Models;

namespace OnDemandHomeService.Controllers
{
    public class NotificationController : Controller
    {
        private readonly AppDbContext _context;

        public NotificationController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult MarkAsRead(int id)
        {
            var notification = _context.Notifications.FirstOrDefault(n => n.NotificationId == id);

            if (notification != null)
            {
                notification.IsRead = true;
                _context.SaveChanges();
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
