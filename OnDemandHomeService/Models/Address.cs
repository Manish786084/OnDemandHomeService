using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnDemandHomeService.Models;

public partial class Address
{
    [Key]
    public int AddressId { get; set; }

    public int UserId { get; set; }

    [StringLength(200)]
    public string? Street { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? State { get; set; }

    [StringLength(10)]
    public string? Pincode { get; set; }

    [StringLength(150)]
    public string? Landmark { get; set; }

    public bool? IsDefault { get; set; }

    [InverseProperty("Address")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [ForeignKey("UserId")]
    [InverseProperty("Addresses")]
    public virtual User User { get; set; } = null!;
}
