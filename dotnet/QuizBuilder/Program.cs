using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using QuizBuilder.Clients;
using QuizBuilder.Database.Adapters;
using QuizBuilder.Database.Migrations;
using QuizBuilder.Services;
using QuizBuilder.Util;
using Refit;
using Serilog;
using Serilog.Formatting.Compact;

namespace QuizBuilder
{

    public static class FluentMigratorHelper
    {
        public static IServiceCollection ConfigureFluentMigrator(this IServiceCollection services) 
        {
            services.AddSingleton<IConventionSet>(new DefaultConventionSet(EnvironmentVars.GetPostgresSchema(), null));
            services.AddFluentMigratorCore()
                .ConfigureRunner(rb =>
                    rb
                        .AddPostgres()
                        .WithGlobalConnectionString($"Host={EnvironmentVars.GetPostgresHost()}; user id={EnvironmentVars.GetPostgresUser()}; password={EnvironmentVars.GetPostgresPassword()}; database={EnvironmentVars.GetDatabaseName()};")
                        .WithRunnerConventions(new MigrationRunnerConventions())
                        .ScanIn(typeof(CreateSchemas).Assembly)
                        .For.Migrations()
                    )
                    .AddLogging(lb =>
                        lb.AddFluentMigratorConsole());

            return services;
        }

        public static void UpdateDatabase(this IServiceProvider services)
        {

            using (var scope = services.CreateScope())
            {
                // Instantiate the runner
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

                // Execute the migrations
                runner.MigrateUp();
            }
        }

        public static void ConfigureDependencies(this IServiceCollection services)
        {

            //services
            services.AddTransient<UserService>();
            services.AddTransient<QuizService>();
            services.AddTransient<QuizQuestionService>();
            services.AddTransient<QuizQuestionChoiceService>();
            services.AddTransient<QuizResponseService>();

            //adapters
            services.AddTransient<QuizAdapter>();
            services.AddTransient<QuizQuestionAdapter>();
            services.AddTransient<QuizQuestionChoiceAdapter>();
            services.AddTransient<QuizResponseAdapter>();

            //refit clients
            services.AddRefitClient<IUserClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri($"http://{EnvironmentVars.GetKeycloakHost()}"));
        }
    }

    public class Startup
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
                .UseStartup<Startup>();
            }).ConfigureAppConfiguration((context, config) =>
            {
                var builtConfig = config.Build();
            });

        public void ConfigureServices(IServiceCollection services) 
        {
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.ConfigureDependencies();
            services.ConfigureFluentMigrator();
            services.AddControllers();
            services.AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o => 
                {
                    o.Authority = $"http://{EnvironmentVars.GetKeycloakHost()}/realms/platformservices";
                    o.RequireHttpsMetadata = false;
                    o.Audience = "webapp";
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = $"http://{EnvironmentVars.GetKeycloakHost()}/realms/platformservices",
                        ValidAudience = "webapp",
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true
                    };
                });

            services.AddAuthorization(options => 
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
            });

        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            app.ApplicationServices.UpdateDatabase();

            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {

               
            }

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }

    public class Program
    {
        static void Main(string[] args)
        {
            Startup.CreateWebHostBuilder(args).Build().Run();
        }
    }
}





