using System.Data.Common;
using System.Text.Json;

namespace SimpleReportModel;

public interface IProvideReportData
{
  IReadOnlyCollection<T> Get<T>(string queryForJson, DbConnection connection, Action<JsonSerializerOptions> setupOptions = null)
    where T : class;
}
