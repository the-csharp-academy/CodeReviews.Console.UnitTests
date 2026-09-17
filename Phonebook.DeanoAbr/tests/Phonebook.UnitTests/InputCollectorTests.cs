using Phonebook.Services;

namespace Phonebook.UnitTests;

public class InputCollectorTests
{
    [Fact]
    public void ReadValid_WhenFirstInputIsValid_ReturnsTrimmedValueWithoutAskingAgain()
    {
        var inputs = new Queue<string?>(new[] { "  Alice  ", "Bob" });
        var rejected = new List<string>();

        var result = InputCollector.ReadValid(() => inputs.Dequeue(), ValidationService.IsValidName, rejected.Add);

        Assert.Equal("Alice", result);
        Assert.Empty(rejected);
        Assert.Single(inputs);            // the second input was never consumed
    }

    [Fact]
    public void ReadValid_WhenInputFailsValidation_KeepsAskingUntilValid()
    {
        var inputs = new Queue<string?>(new[] { "not-an-email", "still bad", "alice@example.com" });
        var rejected = new List<string>();

        var result = InputCollector.ReadValid(() => inputs.Dequeue(), ValidationService.IsValidEmail, rejected.Add);

        Assert.Equal("alice@example.com", result);
        Assert.Empty(inputs);
        Assert.Equal(new[] { "not-an-email", "still bad" }, rejected);
    }

    [Fact]
    public void ReadValid_WhenInvalidInputIsReported_ErrorCallbackSeesTheTrimmedValue()
    {
        var inputs = new Queue<string?>(new[] { "  bogus  ", "alice@example.com" });
        var rejected = new List<string>();

        InputCollector.ReadValid(() => inputs.Dequeue(), ValidationService.IsValidEmail, rejected.Add);

        Assert.Equal(new[] { "bogus" }, rejected);
    }

    [Fact]
    public void ReadValid_WhenInputIsBlank_ReturnsEmptyStringAndDoesNotValidate()
    {
        var inputs = new Queue<string?>(new[] { "   ", "alice@example.com" });
        var rejected = new List<string>();
        var validationCalls = 0;
        bool AlwaysCount(string value) { validationCalls++; return ValidationService.IsValidEmail(value); }

        var result = InputCollector.ReadValid(() => inputs.Dequeue(), AlwaysCount, rejected.Add);

        Assert.Equal(string.Empty, result);
        Assert.Equal(0, validationCalls);
        Assert.Empty(rejected);
        Assert.Single(inputs);
    }

    [Fact]
    public void ReadValid_WhenReaderReturnsNull_TreatsItAsCancellation()
    {
        var inputs = new Queue<string?>(new string?[] { null, "alice@example.com" });

        var result = InputCollector.ReadValid(() => inputs.Dequeue(), ValidationService.IsValidEmail, _ => { });

        Assert.Equal(string.Empty, result);
    }
}
