using App.Metrics;
using App.Metrics.AspNetCore;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;


namespace Grotesque
{
    public class Program
    {
        public static IMetricsRoot Metrics { get; set; }

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            Metrics = new MetricsBuilder()
                .OutputMetrics.AsPrometheusPlainText()
                .Build();

            return WebHost.CreateDefaultBuilder(args)
                .ConfigureMetrics(Metrics)
                .UseMetrics()
                  .UseStartup<Startup>()
                  .Build();
        }
    }
}
