using System.ComponentModel.DataAnnotations;

namespace silvermax.PhoneBook;

public class Contact
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public DateTime AddedAt { get; set; }
}
