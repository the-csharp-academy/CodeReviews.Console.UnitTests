using CodingTracker.kilozdazolik;
using FluentAssertions;
using FluentAssertions.Extensions;

namespace CodingTracker.Tests.Helpers;

public class HelperTests
{
    private readonly Helper _helper;

    public HelperTests()
    {
        _helper = new Helper();
    }
    
    [Fact]
    public void ValidateDate_ValidFormat_ReturnsTrue()
    {
        //Arrange
        string validDate = "08/12/2025 14:30:00";
        DateTime parsedDate;
        
        //Act
        bool result = _helper.ValidateDate(validDate, out parsedDate);
        
        //Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void ValidateDate_InvalidFormat_ReturnsFalse()
    {
        //Arrange
        string invalidDate = "2025.12.01";
        DateTime parsedDate;
        
        //Act
        bool result = _helper.ValidateDate(invalidDate, out parsedDate);
        
        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsSessionDatesValid_ValidFormat_ReturnsTrue()
    {
        //Arrange
        var startDate = 1.March(2025).At(22, 15).AsLocal();
        var endDate = 2.March(2025).At(22, 15).AsLocal();
        
        //Act
        bool result = _helper.IsSessionDatesValid(startDate,endDate);
        
        //Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void IsSessionDatesValid_InvalidFormat_ReturnsFalse()
    {
        //Arrange
        var startDate = 20.March(2025).At(22, 15).AsLocal();
        var endDate = 1.March(2025).At(22, 15).AsLocal();
        
        //Act
        bool result = _helper.IsSessionDatesValid(startDate,endDate);
        
        //Assert
        result.Should().BeFalse();
    }
}
