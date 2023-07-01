using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using System.Text.Json;

namespace SimpleReportModel.Tests;

public class ReportDataProviderTest
{
  private readonly Mock<IProvideJsonResult> jsonResultProviderMock;
  private readonly ReportDataProvider testedInstance;

  public ReportDataProviderTest()
  {
    jsonResultProviderMock = new Mock<IProvideJsonResult>();

    testedInstance = new ReportDataProvider(jsonResultProviderMock.Object);
  }

  [Fact]
  public async Task GetReturnsEmptyWhenNoRows()
  {
    jsonResultProviderMock.Setup(m => m.GetQueryResult(It.IsAny<string>()))
      .ReturnsAsync(() => "[]");

    var returnedData = await testedInstance.Get<EmptyClass>(string.Empty);

    returnedData.Should().NotBeNull();
    returnedData.Should().BeEmpty();
  }

  [Fact]
  public async Task GetReturnsExpectedNumberOfEntities()
  {
    jsonResultProviderMock.Setup(m => m.GetQueryResult(It.IsAny<string>()))
      .ReturnsAsync(() => "[{},{}]");

    var returnedData = await testedInstance.Get<EmptyClass>(string.Empty);

    returnedData.Should().NotBeNullOrEmpty();
    returnedData.Count.Should().Be(2);
  }

  [Fact]
  public async Task GetThrowsOnRootEntity()
  {
    jsonResultProviderMock.Setup(m => m.GetQueryResult(It.IsAny<string>()))
      .ReturnsAsync(() => "{}");

    var action = () => testedInstance.Get<EmptyClass>(string.Empty);

    await action.Should().ThrowAsync<JsonException>();
  }

  public class EmptyClass
  { }
}