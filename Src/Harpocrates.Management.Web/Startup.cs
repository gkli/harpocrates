using System;
//using Harpocrates.Management.Web.Data;
//using Harpocrates.Management.Web.Infrastructure;
//using Harpocrates.Management.Web.Infrastructure.ApplicationUserClaims;
//using Harpocrates.Management.Web.Infrastructure.AppSettingsModels;
using Harpocrates.Management.Web.Models.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Harpocrates.Management.Web
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

            services.AddControllersWithViews();

            services.AddSingleton<Runtime.Common.Configuration.IConfigurationManager, Server.WebServerConfigurationProvider>()
                    .AddTransient<SecretManagement.DataAccess.ISecretMetadataDataAccessProvider>(s =>
                    {
                        Runtime.Common.Configuration.IConfigurationManager cfg = s.GetRequiredService<Runtime.Common.Configuration.IConfigurationManager>();

                        return new SecretManagement.DataAccess.StorageAccount.SecretMetadataStorageAccountDataAccessProvider(
                            cfg.SecretManagementConnectionString, cfg);
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            //app.UseForwardedHeaders();

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //}

            //app.UseStatusCodePagesWithReExecute("/status-code", "?code={0}");

            ////app.UseHttpsRedirection();
            //app.UseStaticFiles();
            //app.UseCookiePolicy();

            //app.UseSession();

            //app.UseRouting();

            //app.UseAuthentication();
            //app.UseAuthorization();

            //app.UseEndpoints(eb =>
            //{
            //    eb.MapRazorPages();
            //    eb.MapControllerRoute("default", "{controller=home}/{action=index}/{id?}");
            //});
        }
    }
}
