using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnDemandHomeService.Models;

public partial class Review
{
    [Key]
    public int ReviewId { get; set; }

    public int BookingId { get; set; }

    public int CustomerId { get; set; }

    public int ProviderId { get; set; }

    public int? Rating { get; set; }

    [StringLength(500)]
    public string? Comment { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("Reviews")]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey("CustomerId")]
    [InverseProperty("ReviewCustomers")]
    public virtual User Customer { get; set; } = null!;

    [ForeignKey("ProviderId")]
    [InverseProperty("ReviewProviders")]
    public virtual User Provider { get; set; } = null!;
}
