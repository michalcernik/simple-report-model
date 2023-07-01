using System.Text.Json;

namespace SimpleReportModel;

public interface IProvideReportData
{
  Task<IReadOnlyCollection<T>> Get<T>(string queryForJson, Action<JsonSerializerOptions> setupOptions = null)
    where T : class;
}
