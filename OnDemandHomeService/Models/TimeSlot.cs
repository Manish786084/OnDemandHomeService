using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnDemandHomeService.Models;

public partial class TimeSlot
{
    [Key]
    public int TimeSlotId { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool? IsActive { get; set; }

    [InverseProperty("TimeSlot")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
