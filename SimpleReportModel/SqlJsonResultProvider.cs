using Microsoft.Data.SqlClient;
using System.Text;

namespace SimpleReportModel;

public class SqlJsonResultProvider : IProvideJsonResult
{
  private readonly Func<SqlConnection> connectionFactory;

  public SqlJsonResultProvider(Func<SqlConnection> connectionFactory)
  {
    this.connectionFactory = connectionFactory;
  }

  public async Task<string> GetQueryResult(string queryForJson)
  {
    using var connection = connectionFactory();
    using var cmd = connection.CreateCommand();
    cmd.CommandText = queryForJson;

    var jsonResult = new StringBuilder();
    var reader = await cmd.ExecuteReaderAsync();

    if (!reader.HasRows)
    {
      jsonResult.Append("[]");
    }
    else
    {
      while (await reader.ReadAsync())
      {
        jsonResult.Append(reader.GetValue(0).ToString());
      }
    }

    return jsonResult.ToString();
  }
}
