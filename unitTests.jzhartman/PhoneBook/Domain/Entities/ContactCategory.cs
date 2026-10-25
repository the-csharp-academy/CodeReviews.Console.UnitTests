namespace PhoneBook.Domain.Entities;

public class ContactCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<Contact> Contacts { get; set; } = new();
}
