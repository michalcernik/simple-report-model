using System.Data.Common;
using System.Text.Json;

namespace SimpleReportModel;

public class ReportDataProvider : IProvideReportData
{
  private readonly IProvideJsonResult jsonProvider;

  public ReportDataProvider(IProvideJsonResult jsonProvider)
  {
    this.jsonProvider = jsonProvider;
  }

  public IReadOnlyCollection<T> Get<T>(string queryForJson, DbConnection connection, Action<JsonSerializerOptions> setupOptions = null)
    where T : class
  {
    var json = jsonProvider.GetQueryResult(queryForJson, connection);
    
    var serializerOptions = new JsonSerializerOptions();
    if (setupOptions != null)
      setupOptions(serializerOptions);
    
    return JsonSerializer.Deserialize<T[]>(json, serializerOptions);
  }
}
