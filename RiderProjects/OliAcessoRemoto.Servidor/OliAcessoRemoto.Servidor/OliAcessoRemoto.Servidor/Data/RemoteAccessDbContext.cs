using Microsoft.EntityFrameworkCore;
using OliAcessoRemoto.Servidor.Models;

namespace OliAcessoRemoto.Servidor.Data;

public class RemoteAccessDbContext : DbContext
{
    public RemoteAccessDbContext(DbContextOptions<RemoteAccessDbContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; }
    public DbSet<ActiveSession> ActiveSessions { get; set; }
    public DbSet<ConnectionLog> ConnectionLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Client entity
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ClientId).IsUnique();
            entity.Property(e => e.ClientId).HasMaxLength(11).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.LastSeen).HasColumnType("datetime2");
        });

        // Configure ActiveSession entity
        modelBuilder.Entity<ActiveSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ControllerClientId).HasMaxLength(11).IsRequired();
            entity.Property(e => e.TargetClientId).HasMaxLength(11).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            entity.Property(e => e.StartTime).HasColumnType("datetime2");
            entity.Property(e => e.EndTime).HasColumnType("datetime2");
        });

        // Configure ConnectionLog entity
        modelBuilder.Entity<ConnectionLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ClientId).HasMaxLength(11).IsRequired();
            entity.Property(e => e.Action).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Timestamp).HasColumnType("datetime2");
        });
    }
}
