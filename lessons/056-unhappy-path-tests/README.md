In today's lesson we'll look at how to write different types of test cases. Up till now we have mostly written tests for the common flow through our code.  Testing the common flow is often called happy path test cases.  Happy path test cases verify that a system functions correctly when given valid inputs and following the expected workflow without errors. These tests focus on confirming that the primary use cases work as intended under ideal conditions.  In contrast, unhappy path test cases explore how the system behaves when faced with invalid inputs, unexpected actions, or failure scenarios. They help ensure the application can handle errors gracefully and maintain stability in less-than-ideal conditions.  When you write tests remember to write both happy and unhappy test cases.


``` cs

```

`dotnet test`

In the coding exercise you will write a database test.

## Main Points
- Testing the common flow is often called happy path test cases. 
- Unhappy path test cases explore how the system behaves when faced with invalid inputs, unexpected actions, or failure scenarios.


## Suggested Coding Exercise
- Have students write unhappy path database test cases.

## Building toward CSTA Standards:
- Develop and use a series of test cases to verify that a program performs according to its design specifications (3B-AP-21) https://www.csteachers.org/page/standards

## Resources
- https://en.wikipedia.org/wiki/Unit_testing
- https://martinfowler.com/bliki/TestPyramid.html
- https://xunit.net/
- https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/test-min-api?view=aspnetcore-9.0
- https://en.wikipedia.org/wiki/Happy_path
