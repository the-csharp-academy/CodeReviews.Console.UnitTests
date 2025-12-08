using CodingTracker.kilozdazolik.Controllers;
using CodingTracker.kilozdazolik.Models;
using CodingTracker.kilozdazolik.Services;
using FakeItEasy;

namespace CodingTracker.Tests.ControllerTests;

public class TrackerControllerTests
{
    private readonly ITrackerService _service;
    private readonly TrackerController _controller;

    public TrackerControllerTests()
    {
        _service = A.Fake<ITrackerService>();
        _controller = new TrackerController(_service);
    }
    
    [Fact]
    public void ViewAllSessions_CallsServiceToGetAllSessions()
    {
        //Arrange
        A.CallTo(() => _service.GetAllSession()).Returns(new List<Tracker>());
        
        //Act
        _controller.ViewAllSessions();

        //Assert
        A.CallTo(() => _service.GetAllSession()).MustHaveHappenedOnceExactly();
    }
}