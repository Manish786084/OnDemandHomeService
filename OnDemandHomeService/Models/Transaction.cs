using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnDemandHomeService.Models;

public partial class Transaction
{
    [Key]
    public int TransactionId { get; set; }

    public int PaymentId { get; set; }

    [StringLength(150)]
    public string? TransactionRef { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("PaymentId")]
    [InverseProperty("Transactions")]
    public virtual Payment Payment { get; set; } = null!;
}
