namespace OnDemandHomeService.ViewModels
{
    public class RegisterVM
    {
        
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; } // Customer or Provider
    }

}
