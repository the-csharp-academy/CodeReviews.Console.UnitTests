using Microsoft.EntityFrameworkCore;
using PhoneBook.Domain.Entities;

namespace PhoneBook.Infrastructure;

public class PhoneBookDbContext : DbContext
{
    public bool DisableSeedData { get; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<ContactCategory> ContactCategories { get; set; }

    public PhoneBookDbContext(DbContextOptions<PhoneBookDbContext> options, bool disableSeedData = false) : base(options)
    {
        DisableSeedData = disableSeedData;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (!DisableSeedData)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PhoneBookDbContext).Assembly);
    }
}
