using codingTracker._0lcm.Interfaces;
using codingTracker._0lcm.Services;
using Moq;

namespace codingTracker._0lcm.NUnitTesting;

public class TimeValidationTests
{
    private Mock<IDateTimeFormats> _dateTimeFormats;
    private ITimeValidationService _timeValidationService;

    [SetUp]
    public void SetUp()
    {
        _dateTimeFormats = new Mock<IDateTimeFormats>();

        _dateTimeFormats.Setup(d => d.HourFormats).Returns(["H:mm", "HH:mm"]);
        _dateTimeFormats.Setup(d => d.DateFormats).Returns(["yyyy-MM-dd", "yyyy-MM-d", "yyyy-M-dd", "yyyy-M-d"]);

        _timeValidationService = new TimeValidationService(_dateTimeFormats.Object);
    }

    [TestCase("2026-1-1", true)]
    [TestCase("2026-01-01", true)]
    [TestCase("2026-01-1", true)]
    [TestCase("2026-1-01", true)]
    [TestCase("2026-31-1", false)]
    [TestCase("26-1-1", false)]
    [TestCase("2026/1/1", false)]
    public void TryValidateDateTime_GivenValidInput_ReturnCorrectBool(string input, bool expectedResult)
    {
        var result = _timeValidationService.TryValidateDateTime(input, out _, out _);

        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [TestCase("10:10", "12:20", true)]
    [TestCase("10:10", "08:20", true)]
    [TestCase("10:10", "12:20", true)]
    [TestCase("1:10", "02:20", true)]
    [TestCase("01:10", "2:20", true)]
    [TestCase("10:01", "12:00", true)]
    [TestCase("10:1", "12:20", false)]
    public void TryValidateStartAndEndTimes_GivenValidInputs_ReturnCorrectBool(string startTimeInput,
        string endTimeInput, bool expectedResult)
    {
        var result =
            _timeValidationService.TryValidateStartAndEndTimePair(startTimeInput, endTimeInput, out _, out _, out _);

        Assert.That(result, Is.EqualTo(expectedResult));
    }
}