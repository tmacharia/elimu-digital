using System;
using DAL.Contexts;
using DAL.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Services;
using Services.Interfaces;
using Services.Implementations;
using Microsoft.Extensions.Caching.Distributed;

namespace web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            Services.Extensions.Uni = SystemName;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            #region DbContext & Identity Configurations
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(DbConnection));

            services.AddDbContext<LePadContext>(options =>
                    options.UseSqlServer(DbConnection));
            services.AddIdentity<AppUser, AppRole>(config => 
            {
                config.SignIn.RequireConfirmedEmail = true;
                config.SecurityStampValidationInterval = TimeSpan.Zero;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            #endregion

            #region Mvc configuration
            services.AddMvc()
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.Formatting = Formatting.Indented;
            });
            #endregion

            #region Site optimizations config
            services.AddResponseCompression();
            services.AddMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
            });
            #endregion

            #region Services to add to DI Container
            services.AddAutoMapper();
            services.AddTransient<ISettingsManager>(s => new SettingsManager(Configuration.GetSection("Settings")));
            services.AddTransient<IEmailSender>(e => new AuthMessageSender(SystemName,AdminEmail,MailPassword));
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<IUploader>(u => new UploaderService(BlobAccount));
            services.AddTransient<IGoogleMapsService>(g => new GoogleMapsService(GoogleMapsApiKey));
            services.AddTransient<IRepositoryFactory, RepositoryFactory>();
            services.AddTransient<IDataManager, DataManager>();
            services.AddTransient<INotificationManager, NotificationManager>();
            services.AddTransient<IProgressTracker, ProgressTracker>();
            services.AddTransient<IFeesManager, FeesManager>();
            #endregion
            
            services.AddSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
                              IHostingEnvironment env, 
                              ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIdentity();
            
            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715
            app.UseSession();
            app.UseOptimizations();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger UI for E-Learning Pad");
            });

            app.UseResponseCompression();
            app.UseStaticFiles();
        }

        #region Private Section
        /// <summary>
        /// Gets connection string to use based on current build configuration
        /// </summary>
        private string DbConnection
        {
            get
            {
                return Configuration.GetConnectionString("Db");
            }
        }
        /// <summary>
        /// Get connection string to blobl storage account
        /// </summary>
        private string BlobAccount
        {
            get
            {
                return Configuration.GetConnectionString("BlobStorage");
            }
        }
        private string SystemName
        {
            get
            {
                return Configuration.GetSection("SMTP")["MailTitle"];
            }
        }
        private string GoogleMapsApiKey
        {
            get
            {
                return Configuration.GetSection("AppSecrets")["GoogleMaps"];
            }
        }
        private string MailPassword
        {
            get
            {
                return Configuration.GetSection("SMTP")["Password"];
            }
        }
        private string AdminEmail
        {
            get
            {
                return Configuration.GetSection("SMTP")["Email"];
            }
        }
        #endregion
    }
}
