using Amazon.S3;
using AssignmentsApi.Extensions;
using AssignmentsApi.Extensions.Json;
using AssignmentsApi.Factories;
using AssignmentsApi.Middleware;
using AssignmentsApi.Services;
using AssignmentsApi.Services.Converters;
using AssignmentsApi.Settings;
using GirtekaCommon.CognitoAuthorization;
using GirtekaCommon.Hosting.Aws;
using GirtekaCommon.Hosting.Aws.Rds;
using GirtekaCommon.Hosting.HealthChecks;
using GirtekaCommon.Hosting.HealthChecks.NpgSql.Extensions;
using GirtekaCommon.Hosting.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;

namespace AssignmentsApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.Configure<PlanningOptions>(Configuration.GetSection("Planning"));
            services.Configure<AcceptancesOptions>(Configuration.GetSection("Acceptances"));
            services.Configure<AuthorizationOptions>(Configuration.GetSection("Authorization"));
            services.Configure<AwsBucketOptions>(Configuration.GetSection("Bucket"));
            services.Configure<PlanningDatabaseOptions>(Configuration.GetSection("Database"));
            services.Configure<PlaningCycleServiceTranformServiceOptions>(
                Configuration.GetSection("PlaningDataTransform"));
            services.Configure<OperationalAreaOptions>(Configuration.GetSection("OperationalAreas"));
            services.Configure<OrderOptions>(Configuration.GetSection("Order"));
            services.Configure<SapClientOptions>(Configuration.GetSection("Sap"));

            services.AddControllers(options => { options.ReturnHttpNotAcceptable = false; })
                .AddNewtonsoftJson(options => options.SerializerSettings.SetupSettings());
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.ConfigureHttpClients(Configuration);

            services.AddAmazonCredentialsProvider(Configuration.GetValue<bool>("Bucket:UseEcs"))
                .AddDefaultEcsAmazonCredentialsProviderOptions()
                .AddOptionsAmazonCredentialsProviderOptions(Configuration.GetSection("Bucket"))
                .AddAmazonServiceAdapter<AmazonS3Client, AwsBucketServiceFactory>()
                .AddAmazonPostgreSqlPasswordProvider()
                .AddPostgreSqlDatabaseConnectionFactory(Configuration.GetSection("Database"));

            services.AddScoped<IPlanningCycleService, PlanningCycleService>();
            services.AddScoped<IDataStorageService, AwsBucketService>();
            services.AddScoped<IPlanningCycleRepository, PlanningCycleRepository>();
            services.AddSingleton<PlanningQueueService>();
            services.AddHostedService<PlanningBackgroundService>();
            services.AddTransient<IVehicleDataConverter, VehicleDataConverter>();
            services.AddTransient<IOrderDataConverter, OrderDataConverter>();

            services.AddHealthChecks()
                .AddUrlGroup(s => new Uri(s.GetRequiredService<IOptions<PlanningOptions>>().Value.HealthCheckUrl),
                    "AxRead", HealthStatus.Degraded, new[] { GirtekaHealthCheckTags.Readiness },
                    TimeSpan.FromSeconds(8))
                .AddNpgSql("postgre_database", failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { GirtekaHealthCheckTags.Liveness });

            services.AddSingleton<PlanningStatusTableCleaner>();
            services.AddDatabaseCleanup(options =>
            {
                options.CleanupInterval = TimeSpan.FromDays(1);
                options.NoWorkDelay = TimeSpan.FromSeconds(5);
                options.StopOnSingleTableError = false;
                options.AddTable("public.planning_cycle_status", serviceProvider =>
                    serviceProvider.GetRequiredService<PlanningStatusTableCleaner>());
            });

            if (Configuration.GetValue<bool>("RouteToAiRouting"))
            {
                services.AddAiRoutingClient(Configuration);
                services.AddScoped<IPlanningCycleStatusService, AiRoutingPlanningCycleStatusService>();
            }
            else
            {
                services.AddScoped<IPlanningCycleStatusService, PlanningCycleStatusService>();
            }

            services.ConfigureSwagger();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddCognitoJwtBearer(Configuration);

            var scopeOptions = Configuration.GetSection(nameof(ScopeOptions)).Get<ScopeOptions>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("assigmentsAPI", new HasScopeRequirement(scopeOptions.Scope));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Assignments API");
                c.RoutePrefix = "swagger";
            });

            app.UseRouting();
            app.UseResponseCompression();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGirtekaHealthChecks(new GirtekaHealthCheckOptions());
            });
        }
    }
}