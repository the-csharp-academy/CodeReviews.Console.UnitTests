using System.ComponentModel.DataAnnotations;
namespace PhoneBook.Models;

public class PhoneNumberDetail
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string Number { get; set; }

    public LabelType Label { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public int ContactId { get; set; }
    public Contact Contact { get; set; } = null!;
}
