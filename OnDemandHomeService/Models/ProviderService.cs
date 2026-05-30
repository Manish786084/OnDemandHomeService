using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnDemandHomeService.Models;

public partial class ProviderService
{
    [Key]
    public int ProviderServiceId { get; set; }

    public int ProviderId { get; set; }

    public int ServiceId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? PriceOverride { get; set; }

    public int? ExperienceYears { get; set; }

    public bool? IsApproved { get; set; }

    [ForeignKey("ProviderId")]
    [InverseProperty("ProviderServices")]
    public virtual User Provider { get; set; } = null!;

    [ForeignKey("ServiceId")]
    [InverseProperty("ProviderServices")]
    public virtual Service Service { get; set; } = null!;
}
