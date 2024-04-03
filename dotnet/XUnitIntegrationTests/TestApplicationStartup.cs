using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using QuizBuilder;
using QuizBuilder.Util;
using Serilog;
using Serilog.Formatting.Compact;

namespace XUnitIntegrationTests
{
    internal class TestApplicationStartup : Startup
    {
        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog((ctx, cfg) =>
            {
                //Override Few of the Configurations
                cfg.Enrich.WithProperty("Application", ctx.HostingEnvironment.ApplicationName)
                    .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName)
                    .WriteTo.Console(new RenderedCompactJsonFormatter());
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel()
                .UseStartup<Startup>()
                .UseUrls($"http://{EnvironmentVars.GetQuizbuilderHostname()}:{EnvironmentVars.GetServicePort()}");
            }).ConfigureAppConfiguration((context, config) =>
            {
                var builtConfig = config.Build();
            });

    }
}
