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

            //services.Configure<ForwardedHeadersOptions>(options =>
            //{
            //    options.ForwardedHeaders =
            //        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

            //});
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed
            //    // for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.Strict;
            //});


            //services.AddDbContext<ApplicationDbContext>(options =>
            //{
            //    options.UseSqlServer(Configuration.GetConnectionString("IdentityDbConnectionString"));
            //});


            //services.AddDefaultIdentity<ApplicationUser>()
            //    .AddRoles<IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            //services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();

            //services.Configure<IdentityOptions>(options =>
            //{
            //    // Default Lockout settings.
            //    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
            //    options.Lockout.MaxFailedAccessAttempts = 5;
            //    options.Lockout.AllowedForNewUsers = true;

            //    // Default Password settings.
            //    options.Password.RequireDigit = true;
            //    options.Password.RequireLowercase = true;
            //    options.Password.RequireNonAlphanumeric = true;
            //    options.Password.RequireUppercase = true;
            //    options.Password.RequiredLength = 6;
            //    options.Password.RequiredUniqueChars = 1;

            //    // Default SignIn settings.
            //    options.SignIn.RequireConfirmedEmail = false;
            //    options.SignIn.RequireConfirmedPhoneNumber = false;

            //    // Default User settings.
            //    options.User.AllowedUserNameCharacters =
            //        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            //    options.User.RequireUniqueEmail = true;
            //});

            //services.ConfigureApplicationCookie(options =>
            //{
            //    options.Cookie.Name = "Harpocrates.Management.AppCookie";
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.IsEssential = true;
            //    // You might want to only set the application cookies over a secure connection:
            //    // options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            //    options.Cookie.SameSite = SameSiteMode.Strict;
            //    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            //    options.SlidingExpiration = true;
            //});

            //services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, options =>
            //{
            //    options.AccessDeniedPath = "/access-denied";
            //    options.LoginPath = "/login";
            //    options.LogoutPath = "/logout";

            //    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
            //});

            //services.AddDataProtection()
            //   .PersistKeysToDbContext<ApplicationDbContext>();

            //services.AddAntiforgery();

            //services.Configure<ScriptTags>(Configuration.GetSection(nameof(ScriptTags)));

            //services.AddControllersWithViews(options =>
            //{
            //    // Slugify routes so that we can use /employee/employee-details/1 instead of
            //    // the default /Employee/EmployeeDetails/1
            //    //
            //    // Using an outbound parameter transformer is a better choice as it also allows
            //    // the creation of correct routes using view helpers
            //    options.Conventions.Add(
            //        new RouteTokenTransformerConvention(
            //            new SlugifyParameterTransformer()));

            //    // Enable Antiforgery feature by default on all controller actions
            //    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            //});


            //services.AddRazorPages(options =>
            //{
            //    options.Conventions.AddAreaPageRoute("Identity", "/Account/Register", "/register");
            //    options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "/login");
            //    options.Conventions.AddAreaPageRoute("Identity", "/Account/Logout", "/logout");
            //    options.Conventions.AddAreaPageRoute("Identity", "/Account/ForgotPassword", "/forgot-password");
            //})
            //   .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            //   .AddSessionStateTempDataProvider();

            //services.AddAuthorization(options =>
            //{
            //    options.DefaultPolicy = new AuthorizationPolicyBuilder()
            //        .RequireAuthenticatedUser()
            //        .Build();
            //});


            //services.AddSession(options =>
            //{
            //    // Set a short timeout for easy testing.
            //    options.IdleTimeout = TimeSpan.FromMinutes(60);
            //    options.Cookie.Name = "Harpocrates.Management.SessionCookie";
            //    // You might want to only set the application cookies over a secure connection:
            //    // options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            //    options.Cookie.SameSite = SameSiteMode.Strict;
            //    options.Cookie.HttpOnly = true;
            //    // Make the session cookie essential
            //    options.Cookie.IsEssential = true;
            //});

            //services.AddHostedService<DbSeederHostedService>();
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
