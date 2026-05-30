using OnDemandHomeService.Models;

namespace OnDemandHomeService.Services
{
    public class NotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        public void Send(int userId, string title, string message)
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
