using PhoneBook.Validation;
namespace PhoneBook.UnitTests
{
    [TestFixture]
    public class InputValidatorTests
    {
        [TestCase("John")]
        [TestCase("Jane Doe")]
        [TestCase("A")]
        public void IsValidName_ShouldReturnTrue_ForValidNames(string input)
        {
            // Act
            var result = InputValidator.IsValidName(input);

            // Assert
            Assert.That(result, Is.True);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void IsValidName_ShouldReturnFalse_ForEmptyNames(string? input)
        {
            // Act
            var result = InputValidator.IsValidName(input!);

            // Assert
            Assert.That(result, Is.False);
        }

        [TestCase("1234567890")]
        [TestCase("+1-800-555-5555")]
        public void IsValidPhoneNumber_ShouldReturnTrue_ForValidPhoneNumbers(string input)
        {
            // Act
            var result = InputValidator.IsValidPhoneNumber(input);
            // Assert
            Assert.That(result, Is.True);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        [TestCase("123")]
        public void IsValidPhoneNumber_ShouldReturnFalse_ForInvalidPhoneNumbers(string? input)
        {
            // Act
            var result = InputValidator.IsValidPhoneNumber(input!);
            // Assert
            Assert.That(result, Is.False);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("invalid-email")]
        public void IsValidEmail_ShouldReturnFalse_ForInvalidEmails(string? input)
        {
            // Act
            var result = InputValidator.IsValidEmail(input!);
            // Assert
            Assert.That(result, Is.False);
        }

        [TestCase("test@example.com")]
        public void IsValidEmail_ShouldReturnTrue_ForValidEmails(string input)
        {
            // Act
            var result = InputValidator.IsValidEmail(input);
            // Assert
            Assert.That(result, Is.True);
        }
    }
}
