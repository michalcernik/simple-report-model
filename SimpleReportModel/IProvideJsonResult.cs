using Microsoft.Data.SqlClient;

namespace SimpleReportModel;

public interface IProvideJsonResult
{
  Task<string> GetQueryResult(string queryForJson);
}
