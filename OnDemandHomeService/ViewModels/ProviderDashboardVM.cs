namespace OnDemandHomeService.ViewModels
{
    public class ProviderDashboardVM
    {
        // CARDS
        public decimal TodayEarnings { get; set; }
        public int CompletedJobs { get; set; }
        public double AvgRating { get; set; }
        public int NewRequests { get; set; }

        // TABLES
        public List<PendingBookingVM> PendingBookings { get; set; }
        public List<TodayScheduleVM> TodaySchedules { get; set; }
        public List<ReviewsVM> Reviews { get; set; }

        // CHART
        public List<decimal> WeeklyEarnings { get; set; }
    }

    public class PendingBookingVM
    {
        public int BookingId { get; set; }
        public string ServiceName { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Time { get; set; }
        public decimal? Amount { get; set; }
    }

    public class TodayScheduleVM
    {
        public string ServiceName { get; set; }
        public string Address { get; set; }
        public string Time { get; set; }
        public decimal? Amount { get; set; }
        public string Status { get; set; }
    }
}