using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnDemandHomeService.Models;

public partial class Payment
{
    [Key]
    public int PaymentId { get; set; }

    public int BookingId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    [StringLength(50)]
    public string? PaymentMethod { get; set; }

    [StringLength(50)]
    public string? PaymentStatus { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PaidAt { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("Payments")]
    public virtual Booking Booking { get; set; } = null!;

    [InverseProperty("Payment")]
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
