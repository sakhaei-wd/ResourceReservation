using Microsoft.EntityFrameworkCore;
using ResourceReservation.Core.Entities;

namespace ResourceReservation.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ایندکس روی فیلدهای پرکاربرد در جستجو - مهم برای گزینه B
        modelBuilder.Entity<Reservation>()
            .HasIndex(r => new { r.ResourceId, r.StartTime, r.EndTime, r.Status })
            .HasDatabaseName("IX_Reservation_Resource_Time_Status");

        modelBuilder.Entity<Reservation>()
            .HasIndex(r => new { r.UserId, r.Status })
            .HasDatabaseName("IX_Reservation_User_Status");

        // Seed data - یک کاربر و چند منبع پیش‌فرض
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "علی احمدی", Email = "ali@company.ir" },
            new User { Id = 2, Name = "مریم رضایی", Email = "maryam@company.ir" }
        );

        modelBuilder.Entity<Resource>().HasData(
            new Resource { Id = 1, Name = "اتاق جلسه A", Type = "اتاق جلسه", MaxConcurrentUsage = 1 },
            new Resource { Id = 2, Name = "اتاق جلسه B", Type = "اتاق جلسه", MaxConcurrentUsage = 1 },
            new Resource { Id = 3, Name = "پروژکتور ۱", Type = "پروژکتور", MaxConcurrentUsage = 1 },
            new Resource { Id = 4, Name = "خودرو شرکتی", Type = "خودرو", MaxConcurrentUsage = 1 }
        );
    }
}