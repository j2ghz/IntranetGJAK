﻿namespace IntranetGJAK
{
    using System.IO;
    using IntranetGJAK.Models;
    using IntranetGJAK.Services;
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Diagnostics.Entity;
    using Microsoft.AspNet.Hosting;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.StaticFiles;
    using Microsoft.Data.Entity;
    using Microsoft.Dnx.Runtime;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.PlatformAbstractions;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using ILogger = Serilog.ILogger;

    public class Startup
    {
        private ILogger log;

        private readonly string envPath;

        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            const string Template = "{Timestamp:HH:mm:ss.fff} [{Level}] [{SourceContext}] {Message}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
#if DNXCORE50
            .WriteTo.ColoredConsole(outputTemplate: Template)
            //.WriteTo.TextWriter(new System.IO.StreamWriter(new System.IO.FileStream(System.IO.Path.Combine(appEnv.ApplicationBasePath, "intranet.log"), System.IO.FileMode.Create)),outputTemplate: Template)
#else
            .WriteTo.LiterateConsole(outputTemplate: Template)
#endif
            .WriteTo.RollingFile(Path.Combine(appEnv.ApplicationBasePath, "intranet-{Date}.log"), outputTemplate: Template)
            .MinimumLevel.Debug()
            .CreateLogger();

            log = Log.ForContext<Startup>();
            log.Information("{AppName} {AppVersion}", appEnv.ApplicationName, appEnv.ApplicationVersion);
            log.Information("{@RuntimeFramework}", appEnv.RuntimeFramework.FullName);
            log.Information("{LogPath}", Path.Combine(appEnv.ApplicationBasePath, "intranet-{Date}.log"));

            envPath = appEnv.ApplicationBasePath;

            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddEntityFramework()
                .AddSqlite()
                .AddDbContext<ApplicationDbContext>(options => options.UseSqlite("Data Source=" + Path.Combine(this.envPath, "database.sqlite")));
            //.AddSqlServer()
            //.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            //services.AddSingleton<IFileRepository, FileRepository>();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.MinimumLevel = LogLevel.Information;
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
                try
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
                    {
                        serviceScope.ServiceProvider.GetService<ApplicationDbContext>()
                             .Database.Migrate();
                    }
                }
                catch { }
            }

            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            var s = new StaticFileOptions { ServeUnknownFileTypes = true };
            app.UseStaticFiles(s);

            app.UseIdentity();

            // To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}