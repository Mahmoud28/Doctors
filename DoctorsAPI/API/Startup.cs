using API.Authorization;
using Seagull.Core.Helper;
using Seagull.Core.Helper.Localization;
using Seagull.Doctors.Data.Interfaces;
using Seagull.Doctors.Data.Repository;
using Seagull.Doctors.Helper.SignalR.Hub;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Seagull.API;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using Seagull.Core.Data;
using Seagull.Core.Data.Interfaces;
using Seagull.Core.Data.Repository;
using Seagull.Doctors.Data.Model;

namespace JwtDemo
{
    public class Startup
    {
        private char[] securityKey;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration _configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string securityKey = "this_is_our_supper_long_security_key_for_token_validation_project_2018_09_07$smesk.in";
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

            services.AddSession();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        //what to validate
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        //setup validate data
                        ValidIssuer = "smesk.in",
                        ValidAudience = "readers",
                        IssuerSigningKey = symmetricSecurityKey
                    };
                });
            RegisterAppService(services);
            RegisterAppInjection(services);
            RegisterAppHelper(services);
            RegisterAppMainRepository(services);
            RegisterAppCustomRepository(services);
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMvc(options =>
            {
                // All endpoints need authorization using our custom authorization filter
                options.Filters.Add(new CustomAuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                options.SerializerSettings.DateFormatString = "dd/MM/yyyy";
            }).AddXmlSerializerFormatters().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Doctors API",
                    Version = "v1"
                });
                c.SwaggerDoc("v1.0", new Info { Title = "Main API v1.0", Version = "v1.0" });
                //var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Seagull.API.xml");
                //c.IncludeXmlComments(filePath);

                // Swagger 2.+ support
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(security);

                c.EnableAnnotations();
                c.OperationFilter<SwaggerHeader>();

            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseSession();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                if (env.IsDevelopment())
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Doctors API");
                else
                    c.SwaggerEndpoint(_configuration["Doctors:SwaggerLiveUrl"], "Doctors Live API");
            });
        }

        #region Injection
        public void RegisterAppService(IServiceCollection services)
        {

            services.AddDbContext<LibraryDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("LibraryDbContext")));
        }

        public void RegisterAppInjection(IServiceCollection services)
        {

            services.AddTransient<IStringLocalizer, JsonStringLocalizer>();

            services.AddTransient<ILocalizationRepository, JsonStringLocalizer>();

        }

        public void RegisterAppHelper(IServiceCollection services)
        {
            services.AddTransient<IStringLocalizer, JsonStringLocalizer>();

            services.AddTransient<ILocalizationRepository, JsonStringLocalizer>();
        }

        public void RegisterAppMainRepository(IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            

            services.AddTransient<IUserRoleMapRepository, UserRoleMapRepository>();

            services.AddTransient<IUserRoleRepository, UserRoleRepository>();

            services.AddTransient<IPermissionRepository, PermissionRepository>();

            services.AddTransient<IPermUserRoleMapRepository, PermUserRoleMapRepository>();
           

        }

        public void RegisterAppCustomRepository(IServiceCollection services)
        {
            services.AddTransient<IEmailTemplateRepositry, EmailTemplateRepositry>();
        }
        #endregion 
    }
}
