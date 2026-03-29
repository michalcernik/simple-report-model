using FluentAssertions;
using Moq;
using Moq.Protected;
using System.Data;
using System.Data.Common;

namespace SimpleReportModel.Tests;

public class SqlJsonResultProviderTest
{
  [Fact]
  public void GetQueryResultConcatenatesMultipleRows()
  {
    var readerMock = new Mock<DbDataReader>();
    readerMock.Setup(r => r.HasRows).Returns(true);
    readerMock.SetupSequence(r => r.Read())
      .Returns(true)
      .Returns(true)
      .Returns(false);
    readerMock.SetupSequence(r => r.GetValue(0))
      .Returns("[{\"Id\":1}")
      .Returns(",{\"Id\":2}]");

    var commandMock = new Mock<DbCommand>();
    commandMock.SetupProperty(c => c.CommandText);
    commandMock.Protected()
      .Setup<DbDataReader>("ExecuteDbDataReader", ItExpr.IsAny<CommandBehavior>())
      .Returns(readerMock.Object);

    var connectionMock = new Mock<DbConnection>();
    connectionMock.Protected()
      .Setup<DbCommand>("CreateDbCommand")
      .Returns(commandMock.Object);

    var provider = new SqlJsonResultProvider();
    var result = provider.GetQueryResult("SELECT * FOR JSON PATH", connectionMock.Object);

    result.Should().Be("[{\"Id\":1},{\"Id\":2}]");
  }
}
