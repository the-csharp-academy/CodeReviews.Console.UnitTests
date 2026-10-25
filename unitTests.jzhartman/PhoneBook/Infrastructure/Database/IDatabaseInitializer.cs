namespace PhoneBook.Infrastructure.Database;

internal interface IDatabaseInitializer
{
    Task EnsureUncategorizedCategoryExistsAsync();
}