using OnDemandHomeService.Models;

namespace OnDemandHomeService.ViewModels
{
    public class HomeVM
    {
        public List<Service> Services { get; set; } = new();

        public List<ProviderService> TopRatedProviders { get; set; } = new();
    }
}
