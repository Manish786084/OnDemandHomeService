using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnDemandHomeService.Models;

public partial class Booking
{
    [Key]
    public int BookingId { get; set; }

    public int CustomerId { get; set; }

    public int? ProviderId { get; set; }

    public int AddressId { get; set; }

    public DateOnly BookingDate { get; set; }

    public int TimeSlotId { get; set; }

    public int StatusId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? TotalAmount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("AddressId")]
    [InverseProperty("Bookings")]
    public virtual Address Address { get; set; } = null!;

    [InverseProperty("Booking")]
    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    [ForeignKey("CustomerId")]
    [InverseProperty("BookingCustomers")]
    public virtual User Customer { get; set; } = null!;

    [InverseProperty("Booking")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [ForeignKey("ProviderId")]
    [InverseProperty("BookingProviders")]
    public virtual User? Provider { get; set; }

    [InverseProperty("Booking")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [ForeignKey("StatusId")]
    [InverseProperty("Bookings")]
    public virtual BookingStatus Status { get; set; } = null!;

    [ForeignKey("TimeSlotId")]
    [InverseProperty("Bookings")]
    public virtual TimeSlot TimeSlot { get; set; } = null!;
}
