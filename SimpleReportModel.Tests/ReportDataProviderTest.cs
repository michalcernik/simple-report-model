using FluentAssertions;
using Moq;
using System.Data.Common;
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
    jsonResultProviderMock.Setup(m => m.GetQueryResult(It.IsAny<string>(), It.IsAny<DbConnection>()))
      .Returns(() => "[]");

    var returnedData = testedInstance.Get<EmptyClass>(string.Empty, null);

    returnedData.Should().NotBeNull();
    returnedData.Should().BeEmpty();
  }

  [Fact]
  public void GetReturnsExpectedNumberOfEntities()
  {
    jsonResultProviderMock.Setup(m => m.GetQueryResult(It.IsAny<string>(), It.IsAny<DbConnection>()))
      .Returns(() => "[{},{}]");

    var returnedData = testedInstance.Get<EmptyClass>(string.Empty, null);

    returnedData.Should().NotBeNullOrEmpty();
    returnedData.Count.Should().Be(2);
  }

  [Fact]
  public void GetThrowsOnRootEntity()
  {
    jsonResultProviderMock.Setup(m => m.GetQueryResult(It.IsAny<string>(), It.IsAny<DbConnection>()))
      .Returns(() => "{}");

    var action = () => testedInstance.Get<EmptyClass>(string.Empty, null);

    action.Should().Throw<JsonException>();
  }

  [Fact]
  public void GetDeserializesProperties()
  {
    jsonResultProviderMock.Setup(m => m.GetQueryResult(It.IsAny<string>(), It.IsAny<DbConnection>()))
      .Returns(() => "[{\"Name\":\"Alice\",\"Age\":30}]");

    var returnedData = testedInstance.Get<PersonClass>(string.Empty, null);

    returnedData.Should().ContainSingle();
    var item = returnedData.Single();
    item.Name.Should().Be("Alice");
    item.Age.Should().Be(30);
  }

  [Fact]
  public void GetAppliesSetupOptions()
  {
    jsonResultProviderMock.Setup(m => m.GetQueryResult(It.IsAny<string>(), It.IsAny<DbConnection>()))
      .Returns(() => "[{\"name\":\"Alice\"}]");

    var returnedData = testedInstance.Get<PersonClass>(
      string.Empty, null,
      opts => opts.PropertyNameCaseInsensitive = true);

    returnedData.Should().ContainSingle();
    returnedData.Single().Name.Should().Be("Alice");
  }

  [Fact]
  public void GetPassesQueryAndConnectionToProvider()
  {
    var expectedQuery = "SELECT * FROM Reports FOR JSON PATH";
    jsonResultProviderMock.Setup(m => m.GetQueryResult(It.IsAny<string>(), It.IsAny<DbConnection>()))
      .Returns("[]");

    testedInstance.Get<EmptyClass>(expectedQuery, null);

    jsonResultProviderMock.Verify(m => m.GetQueryResult(expectedQuery, null), Times.Once);
  }

  public class EmptyClass { }

  public class PersonClass
  {
    public string Name { get; set; }
    public int Age { get; set; }
  }
}