using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnDemandHomeService.Models;

[Table("BookingStatus")]
[Index("StatusName", Name = "UQ__BookingS__05E7698AE1824353", IsUnique = true)]
public partial class BookingStatus
{
    [Key]
    public int StatusId { get; set; }

    [StringLength(50)]
    public string StatusName { get; set; } = null!;

    [InverseProperty("Status")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
