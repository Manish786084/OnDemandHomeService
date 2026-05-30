namespace OnDemandHomeService.ViewModels
{
    public class PaymentVM
    {
        public int BookingId { get; set; }
        public string CustomerName { get; set; }
        public string ServiceName { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
    }
}
