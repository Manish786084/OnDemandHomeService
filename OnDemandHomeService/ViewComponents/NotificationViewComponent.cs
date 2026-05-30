using Microsoft.AspNetCore.Mvc;
using OnDemandHomeService.Models;
using OnDemandHomeService.ViewModels;

namespace OnDemandHomeService.ViewComponents
{
    //public class NotificationViewComponent : ViewComponent
    //{
    //    private readonly AppDbContext _context;

    //    public NotificationViewComponent(AppDbContext context)
    //    {
    //        _context = context;
    //    }

    //    public IViewComponentResult Invoke()
    //    {
    //        var userId = HttpContext.Session.GetInt32("UserId");

    //        if (userId == null)
    //            return View(new List<Notification>());

    //        var notifications = _context.Notifications
    //            .Where(n => n.UserId == userId.Value && n.IsRead == false)
    //            .OrderByDescending(n => n.CreatedAt)
    //            .Take(5)
    //            .ToList();

    //        return View(notifications);
    //    }
    //}
    public class NotificationViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public NotificationViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var vm = new NotificationVM
            {
                Notifications = new List<Notification>(),
                Count = 0
            };

            if (userId == null)
                return View(vm);

            vm.Notifications = _context.Notifications
                .Where(n => n.UserId == userId && n.IsRead == false)
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .ToList();

            vm.Count = _context.Notifications
                .Count(n => n.UserId == userId && n.IsRead == false);

            return View(vm);
        }
    }
}
