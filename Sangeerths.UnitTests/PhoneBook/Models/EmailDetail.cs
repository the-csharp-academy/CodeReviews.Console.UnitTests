using System.ComponentModel.DataAnnotations;
namespace PhoneBook.Models;

public class EmailDetail
{
    public int Id {get; set; }

    [Required]
    [MaxLength(255)]
    public string EmailAddress { get; set; }

    public LabelType Label { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public int ContactId { get; set; }
    public Contact Contact { get; set; } = null!;
}
