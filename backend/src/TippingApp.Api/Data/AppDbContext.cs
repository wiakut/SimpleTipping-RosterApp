using Microsoft.EntityFrameworkCore;
using TippingApp.Api.Models;

namespace TippingApp.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<TipEntry> TipEntries => Set<TipEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Role).HasMaxLength(50);
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(s => s.Employee)
                  .WithMany(e => e.Shifts)
                  .HasForeignKey(s => s.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(s => new { s.EmployeeId, s.Date });
        });

        modelBuilder.Entity<TipEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.Source).HasMaxLength(100);
            entity.HasIndex(t => t.Date);
        });
    }
}
