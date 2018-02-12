using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Contexts;
using DAL.Models;
using Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Services;

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
            
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(DbConnection));

            services.AddDbContext<LePadContext>(options =>
                    options.UseSqlServer(DbConnection));

            services.AddIdentity<AppUser, AppRole>(config => 
                {
                    config.SignIn.RequireConfirmedEmail = true;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc()
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.Formatting = Formatting.Indented;
                    });

            // Add application services
            services.AddTransient<IEmailSender>(e => new AuthMessageSender(MailTitle,AdminEmail,MailPassword));
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<IUploader>(u => new UploaderService(BlobAccount));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
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

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
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
        private string MailTitle
        {
            get
            {
                return Configuration.GetSection("SMTP")["MailTitle"];
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
