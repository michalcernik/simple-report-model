using System.Text.Json;

namespace SimpleReportModel;

public class ReportDataProvider : IProvideReportData
{
  private readonly IProvideJsonResult jsonProvider;

  public ReportDataProvider(IProvideJsonResult jsonProvider)
  {
    this.jsonProvider = jsonProvider;
  }
  
  public async Task<IReadOnlyCollection<T>> Get<T>(string queryForJson, Action<JsonSerializerOptions> setupOptions = null)
    where T : class
  {
    var json = await jsonProvider.GetQueryResult(queryForJson);
    
    var serializerOptions = new JsonSerializerOptions();
    if (setupOptions != null)
      setupOptions(serializerOptions);
    
    return JsonSerializer.Deserialize<T[]>(json, serializerOptions);
  }
}
