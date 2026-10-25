using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhoneBook.Domain.Entities;

namespace PhoneBook.Infrastructure.Configurations;

internal class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");

        builder.HasKey(c => c.Id);

        builder.HasOne(c => c.Category)
            .WithMany(cat => cat.Contacts)
            .HasForeignKey(c => c.CategoryId)
            .IsRequired();

        builder.HasData(new List<Contact>
            {
                new Contact
                {
                    Id = 1,
                    FirstName = "Malcolm",
                    LastName = "Reynolds",
                    PhoneNumber = "1111111111",
                    Email = "browncoat@serenity.com",
                    CategoryId = 1
                },
                new Contact
                {
                    Id = 2,
                    FirstName = "Inara",
                    LastName = "Serra",
                    PhoneNumber = "2222222222",
                    Email = "companion@serenity.com",
                    CategoryId = 1
                },
                new Contact
                {
                    Id = 3,
                    FirstName = "Shepherd",
                    LastName = "Book",
                    PhoneNumber = "3333333333",
                    Email = "book@serenity.com",
                    CategoryId = 1
                },
                new Contact
                {
                    Id = 4,
                    FirstName = "Jayne",
                    LastName = "Cobb",
                    PhoneNumber = "4444444444",
                    Email = "vera@serenity.com",
                    CategoryId = 1
                },
                new Contact
                {
                    Id = 5,
                    FirstName = "Kaylee",
                    LastName = "Frye",
                    PhoneNumber = "5555555555",
                    Email = "mechanic@serenity.com",
                    CategoryId = 1
                },
                new Contact
                {
                    Id = 6,
                    FirstName = "Simon",
                    LastName = "Tam",
                    PhoneNumber = "6666666666",
                    Email = "awesomeDoctor@serenity.com",
                    CategoryId = 1
                },
                new Contact
                {
                    Id = 7,
                    FirstName = "River",
                    LastName = "Tam",
                    PhoneNumber = "7777777777",
                    Email = "miranda@serenity.com",
                    CategoryId = 1
                },
                new Contact
                {
                    Id = 8,
                    FirstName = "Hoban",
                    LastName = "Washburne",
                    PhoneNumber = "8888888",
                    Email = "leafOnTheWind@serenity.com",
                    CategoryId = 1
                },
                new Contact
                {
                    Id = 9,
                    FirstName = "Zoe",
                    LastName = "Washburne",
                    PhoneNumber = "99999999",
                    Email = "browncoats@serenity.com",
                    CategoryId = 1
                }
        });
    }
}
