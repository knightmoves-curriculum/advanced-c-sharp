In today's lesson we'll look at test mocks.  A mock in unit testing is an object that simulates the behavior of real dependencies and allows you to verify specific interactions, such as method calls and parameters. It is valuable when you need to test how a method interacts with its dependencies, ensuring the correct sequence and conditions of calls. Unlike a simple stub, which only provides predefined responses, a mock also verifies that the expected interactions occurred during the test.

1. run `cd src/MyFirstApi`
1. run `dotnet add package Moq`
1. run `cd ../../`

``` cs
using Moq;

namespace MyFirstApiTests;

public class RateLimitingServiceTest
{
    private DateTime currentDateTime;
    private RateLimitingService rateLimitingService;
    private Mock<IDateTimeWrapper> mockDateTimeWrapper;

    public RateLimitingServiceTest(){
        currentDateTime = DateTime.UtcNow;

        mockDateTimeWrapper = new Mock<IDateTimeWrapper>();

        mockDateTimeWrapper.Setup(d => d.UtcNow()).Returns(currentDateTime);

        rateLimitingService = new(mockDateTimeWrapper.Object);
    }

    [Fact]
    public void ShouldAllowRequest_WhenTenthRequestIsReceivedWithinOneMinute(){
        //Arrange
        for (int i = 0; i < 9; i++)
        {
                rateLimitingService.IsRequestAllowed("test");
        }

        //Act
        var isAllowed = rateLimitingService.IsRequestAllowed("test");

        //Assert
        Assert.True(isAllowed);
    }

    [Fact]
    public void ShouldNotAllowRequest_WhenEleventhRequestIsReceivedWithinOneMinute(){
        //Arrange
        for (int i = 0; i < 10; i++)
        {
                rateLimitingService.IsRequestAllowed("test");
        }

        //Act
        var isAllowed = rateLimitingService.IsRequestAllowed("test");

        //Assert
        Assert.False(isAllowed);
    }

    [Fact]
    public void ShouldAllowRequest_WhenEleventhRequestIsReceivedOverOneMinuteLater(){
        //Arrange
        var initialTime = DateTime.Parse("2000-01-01 01:01:01");
        mockDateTimeWrapper.Setup(d => d.UtcNow()).Returns(initialTime);

        for (int i = 0; i < 10; i++)
        {
                rateLimitingService.IsRequestAllowed("test");
        }

        var oneMinuteOneMillisecondLater = initialTime.AddMinutes(1).AddMilliseconds(1);
        mockDateTimeWrapper.Setup(d => d.UtcNow()).Returns(oneMinuteOneMillisecondLater);

        //Act
        var isAllowed = rateLimitingService.IsRequestAllowed("test");

        //Assert
        Assert.True(isAllowed);

        mockDateTimeWrapper.Verify(d => d.UtcNow());
    }
}
```

`dotnet test`

In the coding exercise you will mock a test dependency.

## Main Points
- A mock in unit testing is an object that simulates the behavior of real dependencies and allows you to verify specific interactions, such as method calls and parameters.

## Suggested Coding Exercise
- Have students mock a test dependency.

## Building toward CSTA Standards:
- Develop and use a series of test cases to verify that a program performs according to its design specifications (3B-AP-21) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/Unit_testing
- https://martinfowler.com/bliki/TestPyramid.html
- https://xunit.net/
- https://en.wikipedia.org/wiki/Mock_object

