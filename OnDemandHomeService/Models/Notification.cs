using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnDemandHomeService.Models;

public partial class Notification
{
    [Key]
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    [StringLength(150)]
    public string? Title { get; set; }

    [StringLength(500)]
    public string? Message { get; set; }

    public bool? IsRead { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Notifications")]
    public virtual User User { get; set; } = null!;
}
