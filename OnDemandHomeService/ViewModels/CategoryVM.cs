using OnDemandHomeService.Models;

namespace OnDemandHomeService.ViewModels
{
    public class CategoryVM
    {
        public Category NewCategory { get; set; }
        public List<Category> Categories { get; set; }
        public bool IsEdit { get; set; }
    }
}
