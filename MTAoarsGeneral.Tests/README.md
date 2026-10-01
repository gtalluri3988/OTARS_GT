# MTAoars Unit Tests

This project contains unit tests for the MTAoars application, focusing on testing the Repository layer.

## Project Structure

```
MTAoars.Tests/
├── Properties/
│   └── AssemblyInfo.cs
├── Repositories/
│   └── Maintenance/
│       └── ReferredRepositoryTests.cs
├── TestHelpers/
│   ├── FakeDbSet.cs
│   └── TestObjectContext.cs
├── App.config
├── packages.config
└── MTAoars.Tests.csproj
```

## Test Framework

The project uses the following testing frameworks and libraries:

- **NUnit 2.6.4** - Unit testing framework
- **Moq 4.2.1510.2205** - Mocking framework for creating mock objects
- **Entity Framework 5.0.0** - For database context

## Prerequisites

Before running the tests, ensure you have:

1. Visual Studio 2015 or later (or MSBuild tools)
2. .NET Framework 4.0 or later
3. NUnit Test Runner (Visual Studio extension or NUnit console runner)

## Running Tests

### Option 1: Using Visual Studio

1. Open `MTAoars.sln` in Visual Studio
2. Build the solution (Ctrl+Shift+B)
3. Open Test Explorer (Test > Windows > Test Explorer)
4. Click "Run All" to execute all tests

### Option 2: Using NUnit Console Runner

1. Install NUnit Console Runner via NuGet:
   ```
   nuget install NUnit.Console -Version 2.6.4
   ```

2. Build the test project:
   ```
   msbuild MTAoars.Tests\MTAoars.Tests.csproj
   ```

3. Run the tests:
   ```
   .\NUnit.Console.2.6.4\tools\nunit-console.exe MTAoars.Tests\bin\Debug\MTAoars.Tests.dll
   ```

### Option 3: Using Visual Studio Command Line

```powershell
# Build the solution
msbuild MTAoars.sln /p:Configuration=Debug

# Run tests using vstest.console
vstest.console.exe MTAoars.Tests\bin\Debug\MTAoars.Tests.dll
```

## Test Coverage

### ReferredRepositoryTests

The `ReferredRepositoryTests` class contains comprehensive tests for the `ReferredRepository`:

#### Constructor Tests
- `Constructor_WithValidContext_ShouldCreateInstance` - Verifies repository can be created with valid context
- `Constructor_WithNullContext_ShouldThrowArgumentNullException` - Verifies proper exception handling

#### GetDetail Tests
- `GetDetail_WithExistingICNumber_ShouldReturnReferredDetail` - Tests retrieval by IC number
- `GetDetail_WithNonExistingICNumber_ShouldReturnNull` - Tests null return for invalid IC
- `GetDetail_WithEmptyICNumber_ShouldReturnNull` - Tests empty string handling
- `GetDetail_WithNullICNumber_ShouldReturnNull` - Tests null parameter handling
- `GetDetail_WithMultipleDetails_ShouldReturnFirstMatch` - Tests behavior with multiple matches

#### GetDetailByCompany Tests
- `GetDetailByCompany_WithValidICNumberAndReferredHeaderID_ShouldReturnDetail` - Tests retrieval by IC and header
- `GetDetailByCompany_WithNonExistingReferredHeaderID_ShouldHandleGracefully` - Tests error handling
- `GetDetailByCompany_WithNonExistingICNumber_ShouldReturnNull` - Tests null return
- `GetDetailByCompany_WithDifferentCompanyID_ShouldReturnNull` - Tests company filtering

#### Integration Tests
- `GetDetail_PerformanceTest_WithLargeDataset` - Tests performance with 1000 records
- `Repository_ShouldHandleConcurrentAccess` - Tests concurrent access scenarios

## Writing New Tests

To add tests for other repositories:

1. Create a new test class in the appropriate folder under `Repositories/`
2. Follow the naming convention: `{ClassName}Tests`
3. Use the `[TestFixture]` attribute on the class
4. Use the `[Test]` attribute on test methods
5. Use the `[SetUp]` and `[TearDown]` attributes for initialization/cleanup

Example:

```csharp
[TestFixture]
public class MyRepositoryTests
{
    private Mock<EntityContext> _mockContext;
    private MyRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _mockContext = new Mock<EntityContext>();
        _repository = new MyRepository(_mockContext.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _repository = null;
        _mockContext = null;
    }

    [Test]
    public void MyTest_WithValidData_ShouldReturnExpectedResult()
    {
        // Arrange
        // ... setup test data

        // Act
        var result = _repository.MyMethod();

        // Assert
        Assert.IsNotNull(result);
    }
}
```

## Mocking Best Practices

The test project includes helper methods for creating mock objects:

- Use `Mock<T>` from Moq for creating mock dependencies
- Setup mock behavior using `.Setup()` and `.Returns()`
- Verify method calls using `.Verify()`
- Use `CreateMockObjectSet<T>()` helper method for Entity Framework ObjectSets

## Troubleshooting

### Common Issues

1. **Missing NuGet Packages**
   - Right-click on the solution and select "Restore NuGet Packages"

2. **Build Errors**
   - Ensure all project references are properly set
   - Check that .NET Framework 4.0 is installed

3. **Tests Not Appearing in Test Explorer**
   - Clean and rebuild the solution
   - Restart Visual Studio
   - Install NUnit Test Adapter extension

4. **Entity Framework Errors**
   - The tests use mocked contexts and don't require a database connection
   - Ensure the EntityContext mock is properly configured

## Contributing

When adding new tests:

1. Follow the existing naming conventions
2. Include both positive and negative test cases
3. Test edge cases and error conditions
4. Add comments explaining complex test scenarios
5. Ensure all tests pass before committing

## Notes

- Tests are designed to be independent and can run in any order
- Mock objects are used to avoid database dependencies
- The test project does not require a database connection
- Each test class follows the AAA pattern (Arrange, Act, Assert)

## Future Enhancements

Potential improvements for the test project:

1. Add tests for other repository classes
2. Implement integration tests with a test database
3. Add code coverage reporting
4. Set up continuous integration (CI) pipeline
5. Add performance benchmarking tests
6. Implement data-driven tests using `[TestCase]` attributes

