using Microsoft.Extensions.DependencyInjection;

namespace SimpleReportModel;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddReportData(this IServiceCollection serviceCollection)
  {
    return serviceCollection
      .AddScoped<IProvideJsonResult, SqlJsonResultProvider>()
      .AddScoped<IProvideReportData, ReportDataProvider>();
  }
}