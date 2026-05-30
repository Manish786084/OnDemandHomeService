using Microsoft.AspNetCore.Mvc.Rendering;
using OnDemandHomeService.Models;

namespace OnDemandHomeService.ViewModels
{
    public class ServiceVM
    {
        public Service NewService { get; set; }
        public List<Service> Services { get; set; }

        public List<SelectListItem> CategoryList { get; set; }

        public bool IsEdit { get; set; }
    }
}
