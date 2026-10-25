using Microsoft.EntityFrameworkCore;
using silvermax.PhoneBook.DbAcess;

namespace silvermax.PhoneBook;

public static class DataSeeder
{
    public static async Task SeedAsync(ContactDbContext db)
    {
        await db.Database.MigrateAsync();

        if (await db.Contacts.AnyAsync())
            return;

        var initial = new[]
        {
            new Contact { Id = Guid.NewGuid(), Name = "Alice Smith",  Email = "alice.smith@example.com",  PhoneNumber = "+1-202-555-0140", AddedAt = DateTime.UtcNow },
            new Contact { Id = Guid.NewGuid(), Name = "Bob Jones",    Email = "bob.jones@example.co.uk", PhoneNumber = "+44-20-7946-0958", AddedAt = DateTime.UtcNow },
            new Contact { Id = Guid.NewGuid(), Name = "Carla Mendes", Email = "carla.mendes@example.com.au", PhoneNumber = "+61-2-9374-4000", AddedAt = DateTime.UtcNow },
            new Contact { Id = Guid.NewGuid(), Name = "Dieter Klein", Email = "dieter.klein@example.de", PhoneNumber = "+49-30-9018-200", AddedAt = DateTime.UtcNow },
            new Contact { Id = Guid.NewGuid(), Name = "Élodie Roy",   Email = "elodie.roy@example.fr",    PhoneNumber = "+33-1-4020-3040", AddedAt = DateTime.UtcNow },
            new Contact { Id = Guid.NewGuid(), Name = "Arjun Patel",  Email = "arjun.patel@example.in",   PhoneNumber = "+91-22-2778-1000", AddedAt = DateTime.UtcNow },
            new Contact { Id = Guid.NewGuid(), Name = "Yuki Tanaka",  Email = "yuki.tanaka@example.jp",   PhoneNumber = "+81-3-1234-5678", AddedAt = DateTime.UtcNow },
            new Contact { Id = Guid.NewGuid(), Name = "Lucas Silva",  Email = "lucas.silva@example.com.br", PhoneNumber = "+55-11-99999-0000", AddedAt = DateTime.UtcNow },
            new Contact { Id = Guid.NewGuid(), Name = "Thabo Nkosi",  Email = "thabo.nkosi@example.co.za", PhoneNumber = "+27-21-555-1234", AddedAt = DateTime.UtcNow },
            new Contact { Id = Guid.NewGuid(), Name = "Giulia Romano",Email = "giulia.romano@example.it",  PhoneNumber = "+39-06-698-1234", AddedAt = DateTime.UtcNow }
        };

        await db.Contacts.AddRangeAsync(initial);
        await db.SaveChangesAsync();
    }
}
