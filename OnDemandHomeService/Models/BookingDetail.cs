using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnDemandHomeService.Models;

public partial class BookingDetail
{
    [Key]
    public int BookingDetailId { get; set; }

    public int BookingId { get; set; }

    public int ServiceId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Price { get; set; }

    public int? Quantity { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("BookingDetails")]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey("ServiceId")]
    [InverseProperty("BookingDetails")]
    public virtual Service Service { get; set; } = null!;
}
