using Org.BouncyCastle.Tls;
using silvermax.PhoneBook;
using silvermax.PhoneBook.Dtos;
using Spectre.Console;
using Spectre.Console.Testing;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Phonebook.UnitTests;

public class UserInputTests
{
    [Fact]
    public void GetInfo_WithInvalidEMail_ShowsErrorAndTakesTheNextInput()
    {
        var (result, output) = RunGetInfo("Contact1", "obviously-not-an-email", "Contact1@email.com", "+442071838750");

        Assert.Equal("Contact1@email.com", result.Email);
        Assert.Contains("Please enter a valid email address", output);
    }

    [Theory]
    [InlineData("442071838750")]
    [InlineData("abc")]
    [InlineData("+44123")]
    public void GetInfo_WithInvalidPhone_ShowsErrorAndAcceptTheNextInput(string badNumber)
    {
        var (result, output) = RunGetInfo("Contact1", "Contact1@email.com", badNumber, "+442071838750");

        Assert.Equal("+442071838750", result.PhoneNumber);
        Assert.Contains("Please enter a valid phone number", output);
    }

    [Fact]
    public void GetEmailInfo_ReturnsMailDto()
    {
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter("Contact1");
        console.Input.PushTextWithEnter("This is the header");
        console.Input.PushTextWithEnter("This is the message");
        AnsiConsole.Console = console;

        var result = new UserInput().GetEmailInfo();

        Assert.Equal("Contact1", result.Name);
        Assert.Equal("This is the header", result.Subject);
        Assert.Equal("This is the message", result.Message);
    }

    [Fact]
    public void GetSMSInfo_ReturnsSMSDto()
    {
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter("Contact2");
        console.Input.PushTextWithEnter("This is the message");
        AnsiConsole.Console = console;

        var result = new UserInput().GetSMSInfo();

        Assert.Equal("Contact2", result.Name);
        Assert.Equal("This is the message", result.SMSMessage);
    }

    private (UserInputDto dto, string output) RunGetInfo(params string[] answers)
    {
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;

        foreach (var answer in answers)
            console.Input.PushTextWithEnter(answer);

        AnsiConsole.Console = console;

        var result = new UserInput().GetInfo();

        return (result, console.Output);
    }
}
