using Microsoft.AspNetCore.Mvc.Rendering;
using OnDemandHomeService.Models;

namespace OnDemandHomeService.ViewModels
{
    public class ProviderVM
    {
        public User NewProvider { get; set; }
        public List<User> Providers { get; set; }
        public bool IsEdit { get; set; }
    }
}
