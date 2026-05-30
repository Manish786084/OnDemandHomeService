using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnDemandHomeService.Models;

public partial class Category
{
    [Key]
    public int CategoryId { get; set; }

    [StringLength(100)]
    [Required(ErrorMessage ="Category name is required")]
    public string CategoryName { get; set; } = null!;

    [StringLength(255)]
    public string? Icon { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
