using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Seagull.Core.Data.Interfaces;
using Seagull.Core.Data.Repository;
using Seagull.Core.Data;
using Microsoft.EntityFrameworkCore;
using Seagull.Core.Helper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Seagull.Core.Helper.Localization;
using Seagull.Core.Data.MappingExtension;
using Newtonsoft.Json.Serialization;
using ReflectionIT.Mvc.Paging;
using Newtonsoft.Json;
using Seagull.Core.Helper.SignalR.SeagullHub;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Seagull.Core.Helper.Caching;
using Microsoft.AspNetCore.Http.Features;
using Seagull.Doctors.Data.Interfaces;
using Seagull.Doctors.Data.Repository;
using Seagull.Tqweemco.Scheduling;
using Seagull.Doctors.Helper.SignalR.Hub;
using Hangfire;
using Seagull.Doctors.Data.Hangfire;

namespace Seagull.Core
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
            RegisterAppService(services);

            RegisterAppInjection(services);

            RegisterAppHelper(services);

            RegisterAppMainRepository(services);

            RegisterAppCustomRepository(services);

            services.AddHttpContextAccessor();

            //services.AddCaching();

            services.AddMemoryCache();

            services.AddDistributedMemoryCache();

            services.AddMvc(o =>
            {
                // o.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.DateFormatString = "dd/MM/yyyy";
            }).AddXmlSerializerFormatters();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(1);
                options.Cookie.HttpOnly = true;
            });

            services.AddAutoMapper();

            services.AddPaging();

            services.AddSignalR();

            services.AddHangfire(config =>
                config
                  .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                  .UseSimpleAssemblyNameTypeSerializer()
                  .UseRecommendedSerializerSettings()
                  .UseSqlServerStorage(Configuration.GetConnectionString("LibraryDbContext"), new Hangfire.SqlServer.SqlServerStorageOptions
                  {
                      //CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                      //SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                      //QueuePollInterval = TimeSpan.Zero,
                      //UseRecommendedIsolationLevel = true,
                      //UsePageLocksOnDequeue = true,
                      //DisableGlobalLocks = true
                  }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            app.UseAuthentication();

            app.UseSeagullMiddleware();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                //app.UseExceptionHandler("/Home/Error");
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseHttpStatusCodeExceptionMiddleware();
            //}
            //else
            //{
            //    app.UseHttpStatusCodeExceptionMiddleware();
            //    app.UseExceptionHandler();
            //}

            app.UseStaticFiles();

            //enable session before MVC
            app.UseSession();


            //app.UseMiddleware(middleware: typeof(VisitorCounterMiddleware));



            app.UseMvc(routes =>
            {
                //Register Area
                routes.MapRoute(name: "Admin", template: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

                //routes.MapRoute(name: "APIService", template: "{area:exists}/{controller=APIService}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSignalR(routes =>
            {
                routes.MapHub<SeagullHub>("/linehub");
            });
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.None
            });
            #region decimal 

            //    var cultureInfo = new CultureInfo("vi-VN");
            //    cultureInfo.NumberFormat.CurrencySymbol = "C";
            //    cultureInfo.NumberFormat.CurrencyDecimalDigits = 0;
            //    cultureInfo.NumberFormat.CurrencyDecimalSeparator = ",";
            //    cultureInfo.NumberFormat.CurrencyGroupSeparator = ".";


            //    CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            //    CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            //    var supportedCultures = new[]
            //    {
            //    cultureInfo
            //};

            //    app.UseRequestLocalization(new RequestLocalizationOptions
            //    {
            //        DefaultRequestCulture = new RequestCulture("vi-VN"),
            //        // Formatting numbers, dates, etc.
            //        SupportedCultures = supportedCultures,
            //        // UI strings that we have localized.
            //        SupportedUICultures = supportedCultures
            //    });


            #endregion

            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter(app.ApplicationServices.GetService<IGlobalSettings>()) },
            });

            //DbInitializer.Seed(app);
        }

        #region Injection
        public void RegisterAppService(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                   .AddCookie(options =>
                   {
                       options.LoginPath = "/login/userlogin";
                       //options.CookieName = "Seagull.User.Base";
                   });

            services.Configure<FormOptions>(x => x.ValueCountLimit = 1000000);

            services.AddDbContext<LibraryDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("LibraryDbContext")));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<ICacheServicecs, CacheService>();

            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();

            services.AddTransient<ICommonHelperService, CommonHelperService>();
        }

        public void RegisterAppInjection(IServiceCollection services)
        {
            services.AddTransient<IGlobalSettings, GlobalSettings>();

            services.AddTransient<IStringLocalizer, JsonStringLocalizer>();

            services.AddTransient<ILocalizationRepository, JsonStringLocalizer>();

            services.AddTransient<ISeagullHub, SeagullHub>();
        }

        public void RegisterAppHelper(IServiceCollection services)
        {
            services.AddTransient<IGlobalSettings, GlobalSettings>();

            services.AddTransient<IStringLocalizer, JsonStringLocalizer>();

            services.AddTransient<ILocalizationRepository, JsonStringLocalizer>();
        }

        public void RegisterAppMainRepository(IServiceCollection services)
        {
            // Add scheduled tasks & scheduler
            services.AddScheduler((sender, args) =>
            {
                Console.Write(args.Exception.Message);
                args.SetObserved();
            });
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddTransient<IUserRoleMapRepository, UserRoleMapRepository>();

            services.AddTransient<IUserRoleRepository, UserRoleRepository>();

            services.AddTransient<IPermissionRepository, PermissionRepository>();

            services.AddTransient<IPermUserRoleMapRepository, PermUserRoleMapRepository>();

            services.AddTransient<IEmailTransferRepository, EmailTransferRepository>();
            services.AddTransient<IEmailTransferUserRepository, EmailTransferUserRepository>();

            services.AddTransient<IPushNotificationRepositry, PushNotificationRepositry>();
            services.AddTransient<IUsersNotificationRepositry, UsersNotificationRepositry>();
            services.AddTransient<IMedicationRepository, MedicationRepository>();
            services.AddTransient<IDaysRepository, DaysRepository>();
            services.AddTransient<IPharmacyRepository, PharmacyRepository>();
            services.AddTransient<IBloodTypeRepository, BloodTypeRepository>();
            services.AddTransient<IMedicationFailedRepository, MedicationFailedRepository>();
            services.AddTransient<IDaysRepository, DaysRepository>();
            services.AddTransient<IAllergyRepository, AllergyRepository>();
            services.AddTransient<IMedicationAllergyMapRepository, MedicationAllergyMapRepository>();


            
        }

        public void RegisterAppCustomRepository(IServiceCollection services)
        {
            services.AddTransient<IEmailTemplateRepositry, EmailTemplateRepositry>();
        }
        #endregion 
    }
}
