using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OnDemandHomeService.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }
    public virtual DbSet<BookingDetail> BookingDetails { get; set; }

    public virtual DbSet<BookingStatus> BookingStatuses { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<ProviderDocument> ProviderDocuments { get; set; }

    public virtual DbSet<ProviderService> ProviderServices { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<TimeSlot> TimeSlots { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__Addresse__091C2AFB602636AF");

            entity.Property(e => e.IsDefault).HasDefaultValue(false);

            entity.HasOne(d => d.User).WithMany(p => p.Addresses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Addresses__UserI__412EB0B6");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951AED4907BFA0");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Address).WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__Addres__5BE2A6F2");

            entity.HasOne(d => d.Customer).WithMany(p => p.BookingCustomers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__Custom__59FA5E80");

            entity.HasOne(d => d.Provider).WithMany(p => p.BookingProviders).HasConstraintName("FK__Bookings__Provid__5AEE82B9");

            entity.HasOne(d => d.Status).WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__Status__5DCAEF64");

            entity.HasOne(d => d.TimeSlot).WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__TimeSl__5CD6CB2B");
        });

        modelBuilder.Entity<BookingDetail>(entity =>
        {
            entity.HasKey(e => e.BookingDetailId).HasName("PK__BookingD__8136D45A2D198CA4");

            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookingDe__Booki__619B8048");

            entity.HasOne(d => d.Service).WithMany(p => p.BookingDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookingDe__Servi__628FA481");
        });

        modelBuilder.Entity<BookingStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__BookingS__C8EE20637BF21AC0");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0BBBFAC84D");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E121B05301D");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsRead).HasDefaultValue(false);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__74AE54BC");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A3858D5D002");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__Bookin__656C112C");
        });

        modelBuilder.Entity<ProviderDocument>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PK__Provider__1ABEEF0F09893D41");

            entity.Property(e => e.IsVerified).HasDefaultValue(false);
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Provider).WithMany(p => p.ProviderDocuments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProviderD__Provi__5070F446");
        });

        modelBuilder.Entity<ProviderService>(entity =>
        {
            entity.HasKey(e => e.ProviderServiceId).HasName("PK__Provider__BC3F6609EAAC1AD5");

            entity.Property(e => e.IsApproved).HasDefaultValue(false);

            entity.HasOne(d => d.Provider).WithMany(p => p.ProviderServices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProviderS__Provi__4AB81AF0");

            entity.HasOne(d => d.Service).WithMany(p => p.ProviderServices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProviderS__Servi__4BAC3F29");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79CE52AE1DEE");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Booking).WithMany(p => p.Reviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__Booking__6E01572D");

            entity.HasOne(d => d.Customer).WithMany(p => p.ReviewCustomers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__Custome__6EF57B66");

            entity.HasOne(d => d.Provider).WithMany(p => p.ReviewProviders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__Provide__6FE99F9F");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A37B46F42");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB00A4A9E8B62");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Category).WithMany(p => p.Services)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Services__Catego__46E78A0C");
        });

        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasKey(e => e.TimeSlotId).HasName("PK__TimeSlot__41CC1F32B0D93DE2");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__55433A6B40CF3B22");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Payment).WithMany(p => p.Transactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacti__Payme__693CA210");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C330E0C47");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__3D5E1FD2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
