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
  public void GetReturnsEmptyWhenNoRows()
  {
    jsonResultProviderMock.Setup(m => m.GetQueryResult(It.IsAny<string>(), It.IsAny<SqlConnection>()))
      .Returns(() => "[]");

    var returnedData = testedInstance.Get<EmptyClass>(string.Empty, null);

    returnedData.Should().NotBeNull();
    returnedData.Should().BeEmpty();
  }

  [Fact]
  public void GetReturnsExpectedNumberOfEntities()
  {
    jsonResultProviderMock.Setup(m => m.GetQueryResult(It.IsAny<string>(), It.IsAny<SqlConnection>()))
      .Returns(() => "[{},{}]");

    var returnedData = testedInstance.Get<EmptyClass>(string.Empty, null);

    returnedData.Should().NotBeNullOrEmpty();
    returnedData.Count.Should().Be(2);
  }

  [Fact]
  public void GetThrowsOnRootEntity()
  {
    jsonResultProviderMock.Setup(m => m.GetQueryResult(It.IsAny<string>(), It.IsAny<SqlConnection>()))
      .Returns(() => "{}");

    var action = () => testedInstance.Get<EmptyClass>(string.Empty, null);

    action.Should().Throw<JsonException>();
  }

  public class EmptyClass
  { }
}