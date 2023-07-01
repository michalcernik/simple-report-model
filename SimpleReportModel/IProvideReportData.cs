using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace SimpleReportModel;

public interface IProvideReportData
{
  IReadOnlyCollection<T> Get<T>(string queryForJson, SqlConnection connection, Action<JsonSerializerOptions> setupOptions = null)
    where T : class;
}
