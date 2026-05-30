using OnDemandHomeService.Models;

namespace OnDemandHomeService.ViewModels
{
    public class AdminDashboardVM
    {
        public int NewRequests { get; set; }
        public int OngoingJobs { get; set; }
        public int ActiveProviders { get; set; }
        public decimal TodayRevenue { get; set; }

        public List<Booking> RecentBookings { get; set; } = new();

        public List<TopProviderVM> TopProviders { get; set; } = new();

        public List<string> ServiceLabels { get; set; } = new();
        public List<int> ServiceCounts { get; set; } = new();

        public List<string> MonthlyLabels { get; set; } = new();
        public List<int> MonthlyBookings { get; set; } = new();

        public double AverageRating { get; set; }
        public int PositiveReviews { get; set; }
        public int NegativeReviews { get; set; }
    }

    public class TopProviderVM
    {
        public string ProviderName { get; set; } = "";
        public string ServiceName { get; set; } = "";
        public int TotalJobs { get; set; }
    }
}