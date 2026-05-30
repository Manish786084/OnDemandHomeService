using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnDemandHomeService.Models;

public partial class ProviderDocument
{
    [Key]
    public int DocumentId { get; set; }

    public int ProviderId { get; set; }

    [StringLength(100)]
    public string? DocumentType { get; set; }

    [StringLength(500)]
    public string? FileUrl { get; set; }

    public bool? IsVerified { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UploadedAt { get; set; }

    [ForeignKey("ProviderId")]
    [InverseProperty("ProviderDocuments")]
    public virtual User Provider { get; set; } = null!;
}
