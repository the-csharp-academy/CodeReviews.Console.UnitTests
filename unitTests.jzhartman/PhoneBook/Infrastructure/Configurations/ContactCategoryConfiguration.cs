using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhoneBook.Domain.Entities;

namespace PhoneBook.Infrastructure.Configurations;

internal class ContactCategoryConfiguration : IEntityTypeConfiguration<ContactCategory>
{
    public void Configure(EntityTypeBuilder<ContactCategory> builder)
    {
        builder.ToTable("ContactCategories");

        builder.HasKey(cat => cat.Id);

        builder.HasData(new List<ContactCategory>
            {
                new ContactCategory
                {
                    Id = 1,
                    Name = "Uncategorized"
                },
                new ContactCategory
                {
                    Id = 2,
                    Name = "Family"
                },
                new ContactCategory
                {
                    Id = 3,
                    Name = "Friends"
                },
                new ContactCategory
                {
                    Id = 4,
                    Name = "Work"
                }
        });
    }
}
