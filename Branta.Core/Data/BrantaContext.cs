using Branta.Core.Classes;
using Branta.Core.Data.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Branta.Core.Data;

public class BrantaContext : DbContext
{
    public virtual DbSet<ExtendedKey> ExtendedKey { get; set; }

    public BrantaContext()
    {
    }

    public BrantaContext(DbContextOptions<BrantaContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(@"Data Source=C:\Temp\BrantaCore.db");
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken token = default)
    {
        var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
        Encryption.EncryptEntities(entries);

        return await base.SaveChangesAsync(token);
    }
}
