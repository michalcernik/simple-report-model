using Microsoft.Data.SqlClient;

namespace SimpleReportModel;

public interface IProvideJsonResult
{
  string GetQueryResult(string queryForJson, SqlConnection connection);
}
