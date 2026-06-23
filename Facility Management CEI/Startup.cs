using API.DB;
using API.Enums;
using Facility_Management_CEI.APIs.Models;
using Facility_Management_CEI.IdentityDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Facility_Management_CEI.Configuration;
using Facility_Management_CEI.Services;


namespace Facility_Management_CEI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.


        //------------------------


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("ar")
                };

                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
            });

            services.AddDbContext<IdentityDb.ApplicationDBContext>(options =>
            options.UseSqlServer(
            Configuration.GetConnectionString("ProjectDataBase")));
            //services.AddControllersWithViews();

            services.AddIdentity<LogUser, IdentityRole>(
            options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedAccount = false;

            }).AddEntityFrameworkStores<IdentityDb.ApplicationDBContext>()
             .AddSignInManager<SignInManager<LogUser>>().AddUserManager<UserManager<LogUser>>();

            //////////////////////////////////////////////////

            services.AddControllersWithViews()  // to solve error Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionaryFactory' has been registered.
                .AddViewLocalization()
                .AddJsonOptions(o => o.JsonSerializerOptions
                .ReferenceHandler = ReferenceHandler.Preserve);//to stop the looping in data loading
            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = null);//to stop the looping in data loading
            services.Configure<IfcProcessingOptions>(Configuration.GetSection("IfcProcessing"));
            services.Configure<ViewerOptions>(Configuration.GetSection("Viewer"));
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = Configuration.GetValue<long>("IfcProcessing:MaxUploadBytes", 2147483648);
            });
            services.AddScoped<IIfcProcessingService, IfcProcessingService>();
            services.AddCors(options =>
            {
                options.AddPolicy("AngularDev", builder =>
                {
                    builder.WithOrigins("http://localhost:4200", "https://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            services.AddSwaggerGen();
            //services.AddDbContext<Facility_Management_CEI.IdentityDb.ApplicationDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("FMTest")));
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.


        /// -------------------------------------------------
        /// 

        private async Task CreateAdminLogUserWithRoles(IServiceProvider serviceProvider)
        {

            //initializing custom roles 
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>(); // more enhancement
            var UserManager = serviceProvider.GetRequiredService<UserManager<LogUser>>();

            string[] roleNames = Enum.GetNames(typeof(UserType)); /*{ "AccountManager", "Owner", "Manager", "Supervisor","Inspector","Agent" };*/

            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                // ensure that the role does not exist
                if (!roleExist)
                {
                    //create the roles and seed them to the database: 
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }

            }

            // find the user with the admin email 
            var _user = await UserManager.FindByNameAsync("admin@email");

            // check if the user exists
            var poweruser = new LogUser();
            if (_user == null)
            {
                //Here you could create the super admin who will maintain the web app

                poweruser.FirstName = "Admin";
                poweruser.UserName = "Admin";
                poweruser.UserName = "admin@email";


                var createPowerUser = await UserManager.CreateAsync(poweruser, "Admin!123");
                
                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the role
                    var TestRoleLogUser = await UserManager.AddToRoleAsync(poweruser, "AccountManager");

                }
            }

        }
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IServiceProvider serviceProvider,
            IOptions<RequestLocalizationOptions> localizationOptions)
        {
            CreateAdminLogUserWithRoles(serviceProvider).Wait(); 

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

            app.Use(async (ctx, next) =>
            {
                await next();

                if (ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted)
                {
                    //Re-execute the request so the user gets the error page
                    string originalPath = ctx.Request.Path.Value;
                    ctx.Items["originalPath"] = originalPath;
                    ctx.Request.Path = "/ErrorPages/Error404";
                    await next();
                }
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRequestLocalization(localizationOptions.Value);
            app.UseRouting();
            app.UseCors("AngularDev");
            app.UseAuthentication();
            app.UseAuthorization();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Facility Management CEI API v1");
                });
            }


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");
            });

            var provider = new FileExtensionContentTypeProvider();

            provider.Mappings[".wexBIM"] = "application/octet-stream";

            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });

        }
    }
}

//update-database MyMigration -context Facility_Management_CEI.IdentityDb.ApplicationDBContext
