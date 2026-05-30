using OnDemandHomeService.Models;
using System.Collections.Generic;

namespace OnDemandHomeService.ViewModels
{
    public class ProviderServiceVM
    {
        public List<ProviderService> Services { get; set; } = new List<ProviderService>();
        public ProviderService CurrentService { get; set; } = new ProviderService();
        public bool IsEdit { get; set; } = false;
    }
}
