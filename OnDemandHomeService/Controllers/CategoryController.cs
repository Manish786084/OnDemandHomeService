using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnDemandHomeService.Models;
using OnDemandHomeService.ViewModels;

namespace OnDemandHomeService.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CategoryVM vm)
        {
            _context.Categories.Add(vm.NewCategory);
            _context.SaveChanges();

            return RedirectToAction("ShowCategory");
        }
        // PAGE LOAD
        public IActionResult ShowCategory(int? id)
        {
            var vm = new CategoryVM
            {
                Categories = _context.Categories.ToList()
            };

            if (id != null)
            {
                vm.NewCategory = _context.Categories.Find(id);
                vm.IsEdit = true;
            }
            else
            {
                vm.NewCategory = new Category();
                vm.IsEdit = false;
            }

            return View(vm);
        }



        //[HttpPost]
        //public IActionResult Save(CategoryVM vm)
        //{
        //    if (vm.NewCategory.CategoryId == 0)
        //    {
        //        _context.Categories.Add(vm.NewCategory);
        //        TempData["Success"] = "Category added successfully.";
        //    }
        //    else
        //    {
        //        _context.Categories.Update(vm.NewCategory);
        //        TempData["Success"] = "Category updated successfully.";
        //    }

        //    _context.SaveChanges();

        //    return RedirectToAction("ShowCategory");
        //}
        [HttpPost]
        public IActionResult Save(CategoryVM vm)
        {
            // CHECK DUPLICATE CATEGORY
            bool exists = _context.Categories
                .Any(x => x.CategoryName.ToLower() ==
                     vm.NewCategory.CategoryName.ToLower()
                     && x.CategoryId != vm.NewCategory.CategoryId);

            if (exists)
            {
                TempData["Error"] = "Category already exists.";
                return RedirectToAction("ShowCategory");
            }

            if (vm.NewCategory.CategoryId == 0)
            {
                _context.Categories.Add(vm.NewCategory);

                TempData["Success"] = "Category added successfully.";
            }
            else
            {
                _context.Categories.Update(vm.NewCategory);

                TempData["Success"] = "Category updated successfully.";
            }

            _context.SaveChanges();

            return RedirectToAction("ShowCategory");
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var cat = _context.Categories.Find(id);

            if (cat == null)
            {
                return RedirectToAction("ShowCategory");
            }

            // Check if category is used in Services table
            bool hasServices = _context.Services.Any(s => s.CategoryId == id);

            if (hasServices)
            {
                TempData["Error"] = "Cannot delete category because services exist under this category.";
                return RedirectToAction("ShowCategory");
            }

            _context.Categories.Remove(cat);
            _context.SaveChanges();

            TempData["Success"] = "Category deleted successfully.";

            return RedirectToAction("ShowCategory");
        }

    }
}
