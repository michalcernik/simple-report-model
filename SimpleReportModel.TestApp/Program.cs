using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleReportModel;
using SimpleReportModel.TestApp;

internal class Program
{
  private static async Task Main(string[] args)
  {
    string query = """
  select 
    p.id AS 'Parent.Id'
    , p.name AS 'Parent.Name'
    , p.nickname AS 'Parent.NickName'
    , k.id AS 'Kid.Id'
    , k.name AS 'Kid.Name'
    , k.nickname AS 'Kid.NickName'
  from parent as p
    left outer join child as k
      on k.parent_id = p.id
  FOR JSON PATH
  """;

    string grouped = """
select 
  p.id AS 'Parent.Id'
  , p.name AS 'Parent.Name'
  , p.nickname AS 'Parent.NickName'
  , (SELECT
      k.id
      , k.name
      , k.nickname
    FROM child as k
    WHERE k.parent_id = p.id
    FOR JSON PATH ) as Kids
from parent as p
FOR JSON PATH
select 
  p.id AS 'Parent.Id'
  , p.name AS 'Parent.Name'
  , p.nickname AS 'Parent.NickName'
  , (SELECT
      k.id
      , k.name
      , k.nickname
    FROM child as k
    WHERE k.parent_id = p.id
    FOR JSON PATH ) as Kids
from parent as p
FOR JSON PATH
""";

    ServiceProvider serviceProvider = RegisterServices();

    using ILoggerFactory loggerFactory = BuildLoggerFactory();
    var logger = loggerFactory.CreateLogger<Program>();

    logger.LogInformation("Starting application");

    var dataProvider = serviceProvider.GetRequiredService<IProvideReportData>();

    var relations = await dataProvider.Get<Relation>(grouped,
      options =>
      {
        options.IncludeFields = true;
        options.PropertyNameCaseInsensitive = true;
      });

    foreach (var relation in relations)
    {
      logger.LogDebug($"{relation.Parent.Name}");
      foreach (var kid in relation.Kids)
        logger.LogDebug($"\t{kid.Name}");
    }

    logger.LogInformation("Closing application");
  }

  private static ILoggerFactory BuildLoggerFactory()
  {
    return LoggerFactory.Create(builder =>
    {
      builder
          .AddFilter("Microsoft", LogLevel.Warning)
          .AddFilter("System", LogLevel.Warning)
          .AddFilter("Program", LogLevel.Debug)
          .AddConsole();
    });
  }

  private static ServiceProvider RegisterServices()
  {
    return new ServiceCollection()
        .AddLogging()
        .AddReportData()
        .AddSqlConnection()
        .BuildServiceProvider();
  }
}

public static class ServiceCollectionExtensions
{
  const string connectionString = "Integrated Security=true;Initial Catalog=json;Server=localhost;TrustServerCertificate=true;";
  public static IServiceCollection AddSqlConnection(this IServiceCollection services)
  {
    services.AddScoped<Func<SqlConnection>>(serviceProvider =>
    () =>
    {
      var connection = new SqlConnection(connectionString);
      connection.Open();
      return connection;
    });
    return services;
  }
}
