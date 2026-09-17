using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Phonebook.Services;

public static partial class ValidationService
{
    // Accepted shapes: +27 82 123 4567 | 082 123 4567 | (011) 555-0198 | 555-1234
    [GeneratedRegex(@"^\+?[0-9\s\-()]{7,20}$")]
    private static partial Regex PhoneFormatRegex();

    public static bool IsValidName(string name)
        => !string.IsNullOrWhiteSpace(name);

    public static bool IsValidEmail(string email)
        => MailAddress.TryCreate(email, out _);

    public static bool IsValidPhoneNumber(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return false;
        if (!PhoneFormatRegex().IsMatch(phone)) return false;

        // Separators are allowed, but the number must contain 7–15 real digits
        int digitCount = phone.Count(char.IsDigit);
        return digitCount is >= 7 and <= 15;
    }
}
