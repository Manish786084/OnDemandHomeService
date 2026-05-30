using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnDemandHomeService.Models;

[Index("Phone", Name = "UQ__Users__5C7E359E6F9DF149", IsUnique = true)]
[Index("Email", Name = "UQ__Users__A9D105343271FA80", IsUnique = true)]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(15)]
    public string Phone { get; set; } = null!;

    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    public int RoleId { get; set; }

    public bool? IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    [InverseProperty("Customer")]
    public virtual ICollection<Booking> BookingCustomers { get; set; } = new List<Booking>();

    [InverseProperty("Provider")]
    public virtual ICollection<Booking> BookingProviders { get; set; } = new List<Booking>();

    [InverseProperty("User")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [InverseProperty("Provider")]
    public virtual ICollection<ProviderDocument> ProviderDocuments { get; set; } = new List<ProviderDocument>();

    [InverseProperty("Provider")]
    public virtual ICollection<ProviderService> ProviderServices { get; set; } = new List<ProviderService>();

    [InverseProperty("Customer")]
    public virtual ICollection<Review> ReviewCustomers { get; set; } = new List<Review>();

    [InverseProperty("Provider")]
    public virtual ICollection<Review> ReviewProviders { get; set; } = new List<Review>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role Role { get; set; } = null!;
}
