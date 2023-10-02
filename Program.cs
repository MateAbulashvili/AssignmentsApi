using GirtekaCommon.CognitoAuthorization;
using GirtekaCommon.Hosting.Utilities.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AssignmentsApi.UnitTests")]

namespace AssignmentsApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost = CreateHostBuilder(args).Build();
            var logger = webHost.Services.GetService<ILogger<Program>>();
            try
            {
                IssuerSigningKeyResolverHelper.InitHttpClient(webHost.Services.GetService<IHttpClientFactory>());

                logger.LogInformation($"Application started. version: {Assembly.GetExecutingAssembly().GetName().Version}");
                webHost.Run();
            }
            catch (Exception exception)
            {
                logger.LogCritical(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseGirtekaSentry();
                })
                .AddGirtekaLogging()
                .AddGirtekaSentryLogging();
    }
}