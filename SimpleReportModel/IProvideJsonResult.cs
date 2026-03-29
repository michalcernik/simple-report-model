using System.Data.Common;

namespace SimpleReportModel;

public interface IProvideJsonResult
{
  string GetQueryResult(string queryForJson, DbConnection connection);
}
