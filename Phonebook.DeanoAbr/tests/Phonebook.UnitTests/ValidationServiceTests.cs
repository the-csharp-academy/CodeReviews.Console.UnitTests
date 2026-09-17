using Phonebook.Services;

namespace Phonebook.UnitTests;

public class ValidationServiceTests
{
    [Theory]
    [InlineData("Alice", true)]
    [InlineData("Alice van der Merwe", true)]
    [InlineData("  Alice  ", true)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData(null, false)]
    public void IsValidName_WhenGivenName_ReturnsExpected(string? name, bool expected)
        => Assert.Equal(expected, ValidationService.IsValidName(name!));

    [Theory]
    [InlineData("alice@example.com", true)]
    [InlineData("bob.smith+tag@sub.example.co.za", true)]
    [InlineData("not-an-email", false)]
    [InlineData("alice@", false)]
    [InlineData("alice at example.com", false)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData(null, false)]
    public void IsValidEmail_WhenGivenEmail_ReturnsExpected(string? email, bool expected)
        => Assert.Equal(expected, ValidationService.IsValidEmail(email!));

    [Theory]
    [InlineData("+27 82 123 4567", true)]
    [InlineData("0821234567", true)]
    [InlineData("(011) 555-0198", true)]
    [InlineData("555-1234", true)]
    [InlineData(" 082 123 4567 ", true)]            // documents that trimming is the caller's job
    [InlineData("123", false)]                       // fewer than 7 digits
    [InlineData("1234567890123456", false)]          // more than 15 digits
    [InlineData("abc", false)]
    [InlineData("+27 82 123 4567 ext 9", false)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData(null, false)]
    public void IsValidPhoneNumber_WhenGivenPhoneNumber_ReturnsExpected(string? phone, bool expected)
        => Assert.Equal(expected, ValidationService.IsValidPhoneNumber(phone!));
}
