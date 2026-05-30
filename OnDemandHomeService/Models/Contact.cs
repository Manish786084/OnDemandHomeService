using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnDemandHomeService.Models
{
    public class Contact
    {
        [Key]
        public int ContactId { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only letters allowed")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [StringLength(150)]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = null!;

        [StringLength(20)]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Subject is required")]
        [StringLength(100)]
        public string Subject { get; set; } = null!;

        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; } = null!;

        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
    }
}