using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnDemandHomeService.Models;

public partial class Service
{
    [Key]
    [Required]
    public int ServiceId { get; set; }
    [Required]
    public int CategoryId { get; set; }
    [Required]
    [StringLength(150)]
    public string ServiceName { get; set; } = null!;
    
    [StringLength(500)]
    public string? Description { get; set; }
    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal BasePrice { get; set; }

    public int? DurationMinutes { get; set; }

    public bool IsActive { get; set; }

    [ValidateNever]
    public virtual Category Category { get; set; } = null!;

    [ValidateNever]
    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    [ValidateNever]
    public virtual ICollection<ProviderService> ProviderServices { get; set; } = new List<ProviderService>();
    //[ValidateNever]
    //public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}