using Microsoft.Data.SqlClient;
using Shouldly;
using System.Text.Json;

namespace SimpleReportModel.Tests;

public class SqlJsonResultProviderTest
{
  private const string ConnectionString = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;";

  [Fact]
  public void GetQueryResultConcatenatesMultipleRows()
  {
    // SQL Server splits JSON output at ~2033 bytes per row.
    // REPLICATE('x', 2000) per row forces the total output to exceed that threshold.
    var query = @"
      SELECT REPLICATE('x', 2000) AS Data
      FROM (VALUES (1),(2)) AS t(n)
      FOR JSON PATH";

    using var connection = new SqlConnection(ConnectionString);
    connection.Open();

    var provider = new SqlJsonResultProvider();
    var result = provider.GetQueryResult(query, connection);

    var parsed = JsonSerializer.Deserialize<JsonElement>(result);
    parsed.ValueKind.ShouldBe(JsonValueKind.Array);
    parsed.GetArrayLength().ShouldBe(2);
  }

  [Fact]
  public void GetQueryResultReturnsEmptyArrayWhenNoRows()
  {
    var query = "SELECT 1 AS Id WHERE 1 = 0 FOR JSON PATH";

    using var connection = new SqlConnection(ConnectionString);
    connection.Open();

    var provider = new SqlJsonResultProvider();
    var result = provider.GetQueryResult(query, connection);

    result.ShouldBe("[]");
  }
}
