using CodingTracker.Niasua.UserInput;

namespace CodingTracker.Tests;

public class ValidatorTests
{
    [Theory]
    [InlineData("24-10-2004 00:00", true)] // valid
    [InlineData("24/10/2004", false)] // invalid
    [InlineData("Just letters", false)] // invalid
    [InlineData("32-10-2004 00:00", false)] // invalid
    public void IsValidDateTime_ValidatesInput_Correctly(string input, bool expected)
    {
        // Arrange
        string dateTimeFormat = "dd-MM-yyyy HH:mm";

        // Act  
        var result = Validator.IsValidDateTime(input, dateTimeFormat, out DateTime date);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("24-10-2004 10:00", "24-10-2004 12:00", true)]
    [InlineData("24-10-2004 10:00", "24-10-2004 10:00", true)]
    [InlineData("24-10-2004 12:00", "24-10-2004 10:00", false)]
    public void IsValidSessionTimes_ValidatesLogic_Correctly(string startStr, string endStr, bool expected)
    {
        // Arrange
        var start = DateTime.Parse(startStr);
        var end = DateTime.Parse(endStr);

        // Act
        var result = Validator.IsValidSessionTimes(start, end);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("3", 3, true)]
    [InlineData("0", 0, true)]
    [InlineData("abc", 0, false)]
    [InlineData("12.5", 0, false)]
    public void IsValidInt_ValidatesLogic_Correctly(string input, int expectedId, bool expectedResult)
    {
        // Act 
        var result = Validator.IsValidInt(input, out int id);

        // Assert
        Assert.Equal(expectedResult, result);

        // int verification
        if (expectedResult)
        {
            Assert.Equal(expectedId, id);
        }
    }

    [Theory]
    [InlineData("ASC", true)]
    [InlineData("DESC", true)]
    [InlineData("XYZ", false)]
    public void IsValidOrder_ValidatesLogic_Correctly(string input, bool expected)
    {
        var result = Validator.IsValidOrder(input);

        Assert.Equal(expected, result);
    }
}