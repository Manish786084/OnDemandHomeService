namespace OnDemandHomeService.ViewModels
{
    public class ProviderBookingVM
    {
        public int BookingId { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string ServiceNames { get; set; }
        public string BookingDate { get; set; }
        public string TimeSlot { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public decimal? TotalAmount { get; set; }
    }

}
