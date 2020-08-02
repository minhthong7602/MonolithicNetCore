using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NetCore.AutoRegisterDi;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using MonolithicNetCore.Common;
using MonolithicNetCore.Common.Caching;
using MonolithicNetCore.Common.Security.Jwt;
using MonolithicNetCore.Data;
using MonolithicNetCore.Data.Infrastructure;
using MonolithicNetCore.Data.Repository;
using MonolithicNetCore.Service;
using MonolithicNetCore.Web.ScheduleJob;
using MonolithicNetCore.Web.ScheduleJob.Job;
using MonolithicNetCore.Web.SignalHub;

namespace MonolithicNetCore
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
            services.AddDistributedMemoryCache();
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
            });
            services.AddMvc().AddSessionStateTempDataProvider();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
               .AddCookie(options =>
               {
                   options.AccessDeniedPath = "/";
                   options.LoginPath = "/";// Cookie settings
                   options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
                   options.SlidingExpiration = true;
                   options.Events.OnRedirectToLogin = (context) =>
                   {
                       context.Response.StatusCode = 401;
                       return Task.CompletedTask;
                   };
               })
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = ConfigAppSetting.JwtIssuer,
                       ValidAudience = ConfigAppSetting.JwtAudience,
                       IssuerSigningKey = JwtSecurityKey.Create(ConfigAppSetting.JwtSigningKey)
                   };
               });
            services.AddAuthorization();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins("*")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            services.AddSingleton<IMemoryCacheManager, MemoryCacheManager>();
            services.AddDbContext<BaseContext>();
            services.AddScoped<IDbFactory, DbFactory>();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins("*")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddMvc().AddJsonOptions(
            //options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore

            //);
            var assemblyToScan = Assembly.GetExecutingAssembly();
            services.RegisterAssemblyPublicNonGenericClasses(typeof(UserRepository).Assembly)
                .Where(c => c.Name.EndsWith("Repository"))
                .AsPublicImplementedInterfaces();

            services.RegisterAssemblyPublicNonGenericClasses(typeof(UserService).Assembly)
               .Where(c => c.Name.EndsWith("Service"))
               .AsPublicImplementedInterfaces();
            services.AddControllersWithViews();

            // Add Quartz services
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            // Add our job
            services.AddSingleton<BackUpDatabaseJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(BackUpDatabaseJob),
                cronExpression: ConfigAppSetting.JobBackupTimeDriver)); // run every 5 seconds
            services.AddHostedService<QuartzHostedService>();

            // Add hub server
            services.AddSignalR();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<HubServer>("/hubserver");
                endpoints.MapHub<LogServer>("/hublog");
            });
        }
    }
}
