using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnDemandHomeService.Models;
using OnDemandHomeService.ViewModels;


namespace OnDemandHomeService.Controllers
{
  
    public class ServiceController : Controller
    {
        private readonly AppDbContext _context;

        public ServiceController(AppDbContext context)
        {
            _context = context;
        }


        public IActionResult ShowService(int? id)
        {
            var vm = new ServiceVM
            {
                Services = _context.Services
                    .Include(s => s.Category)
                    .ToList(),

                CategoryList = _context.Categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName
                    }).ToList()
            };

            if (id != null)
            {
                var service = _context.Services.Find(id);
                if (service != null)
                {
                    // Ensure non-null
                    service.IsActive = service.IsActive;
                    vm.NewService = service;
                    vm.IsEdit = true;
                }
                else
                {
                    vm.NewService = new Service { IsActive = true };
                    vm.IsEdit = false;
                }
            }
            else
            {
                vm.NewService = new Service { IsActive = true };
                vm.IsEdit = false;
            }

            return View(vm);
        }



        [HttpPost]
        public IActionResult Save(ServiceVM vm)
        {
            if (vm.NewService.ServiceId == 0)
                _context.Services.Add(vm.NewService);
            else
                _context.Services.Update(vm.NewService);

            _context.SaveChanges();

            return RedirectToAction("ShowService");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var service = _context.Services.Find(id);

            if (service != null)
            {
                _context.Services.Remove(service);
                _context.SaveChanges();
            }

            return RedirectToAction("ShowService");
        }
    }

}
