using System.ComponentModel.DataAnnotations;
namespace PhoneBook.Models;

public class Contact
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? OrganizationName { get; set; }

    [MaxLength(100)]
    public string? JobTitle { get; set; }

    public string? Notes { get; set; }

    public List<PhoneNumberDetail> PhoneNumbers { get; set; } = new();
    public List<EmailDetail> Emails { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
