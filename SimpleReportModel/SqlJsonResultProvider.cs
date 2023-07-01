using Microsoft.Data.SqlClient;
using System.Text;

namespace SimpleReportModel;

public class SqlJsonResultProvider : IProvideJsonResult
{
  public string GetQueryResult(string queryForJson, SqlConnection connection)
  {
    using (var cmd = connection.CreateCommand())
    {
      cmd.CommandText = queryForJson;

      var jsonResult = new StringBuilder();
      var reader = cmd.ExecuteReader();

      if (!reader.HasRows)
      {
        jsonResult.Append("[]");
      }
      else
      {
        while (reader.Read())
        {
          jsonResult.Append(reader.GetValue(0).ToString());
        }
      }
      return jsonResult.ToString();
    }
  }
}
