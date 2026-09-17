using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Phonebook.Models;

namespace Phonebook.Data;

public class AppDbContext : DbContext
{
    public DbSet<Contact> Contacts => Set<Contact>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder
                .UseSqlite("Data Source=phonebook.db")
                // Assignment tip: print every SQL command EF sends to the database
                .LogTo(
                    Console.WriteLine,
                    new[] { DbLoggerCategory.Database.Command.Name },
                    LogLevel.Information)
                // Learning only: also print parameter values; remove in a real app
                .EnableSensitiveDataLogging();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Email).IsRequired().HasMaxLength(255);
            entity.Property(c => c.PhoneNumber).IsRequired().HasMaxLength(30);
            // Store the category as a human-readable string ("Family", not "0")
            entity.Property(c => c.Category).HasConversion<string>();

            // Code-First seeding: EF inserts these rows when it creates the schema
            entity.HasData(
                new Contact { Id = 1, Name = "Alice van der Merwe", Email = "alice@example.com", PhoneNumber = "+27 82 123 4567", Category = ContactCategory.Family },
                new Contact { Id = 2, Name = "Bob Smith", Email = "bob@example.com", PhoneNumber = "082 555 0147", Category = ContactCategory.Work },
                new Contact { Id = 3, Name = "Carol Nkosi", Email = "carol@example.com", PhoneNumber = "(011) 555-0198", Category = ContactCategory.Friends }
            );
        });
    }
}
