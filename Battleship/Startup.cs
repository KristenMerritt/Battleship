using System.IO;
using Battleship.Config;
using Battleship.Repos;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Battleship
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
	        Configuration = new ConfigurationBuilder()
		        .SetBasePath(env.ContentRootPath)
		        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
		        .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
			        optional: true)
		        .AddEnvironmentVariables()
		        .Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
	        services.AddScoped<DataContext>();
            services.AddScoped<ChallengeRepo>();
            services.AddScoped<ChatRepo>();
            services.AddScoped<GameRepo>();
            services.AddScoped<PlayerRepo>();
            services.AddScoped<ShipLocationRepo>();
            services.AddScoped<ShipRepo>();
            services.AddScoped<ShipTypeRepo>();
            services.AddScoped<ShotRepo>();
            services.AddScoped<BoardRepo>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "login")),
                RequestPath = new PathString("/login")
            });

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "game")),
                RequestPath = new PathString("/game")
            });

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "createAccount")),
                RequestPath = new PathString("/createAccount")
            });

#if DEBUG
            TelemetryConfiguration.Active.DisableTelemetry = true;
#endif
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                   ForwardedHeaders.XForwardedProto
            });
        }
    }
}
