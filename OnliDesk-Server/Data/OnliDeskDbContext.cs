using Microsoft.EntityFrameworkCore;
using OnliDesk.Server.Models;

namespace OnliDesk.Server.Data;

public class OnliDeskDbContext : DbContext
{
    public OnliDeskDbContext(DbContextOptions<OnliDeskDbContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; }
    public DbSet<ActiveSession> ActiveSessions { get; set; }
    public DbSet<ConnectionLog> ConnectionLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Client configuration
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ClientId).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ComputerName).HasMaxLength(100);
            entity.Property(e => e.UserName).HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.Version).HasMaxLength(20);
            entity.HasIndex(e => e.ClientId).IsUnique();
        });

        // ActiveSession configuration
        modelBuilder.Entity<ActiveSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ClientId).IsRequired().HasMaxLength(20);
            entity.Property(e => e.ConnectionId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
        });

        // ConnectionLog configuration
        modelBuilder.Entity<ConnectionLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ClientId).IsRequired().HasMaxLength(20);
            entity.Property(e => e.TargetClientId).HasMaxLength(20);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
        });
    }
}
