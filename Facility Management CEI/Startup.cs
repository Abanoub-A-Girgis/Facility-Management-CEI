using API.DB;
using API.Enums;
using Facility_Management_CEI.APIs.Models;
using Facility_Management_CEI.IdentityDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(item => item.Value.Errors.Count > 0)
                        .SelectMany(item => item.Value.Errors.Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage) ? "The request is invalid." : error.ErrorMessage))
                        .ToList();

                    return new BadRequestObjectResult(Facility_Management_CEI.ApiLayer.Dtos.ApiResponse<object>.Fail(string.Join(" ", errors)));
                };
            });

            services.AddDbContext<IdentityDb.ApplicationDBContext>(options =>
            options.UseSqlServer(
            Configuration.GetConnectionString("ProjectDataBase")));
            //services.AddControllersWithViews();

            services.AddIdentity<LogUser, IdentityRole>(
            options =>
            {
                options.Password.RequireDigit = Configuration.GetValue("Identity:Password:RequireDigit", true);
                options.Password.RequireLowercase = Configuration.GetValue("Identity:Password:RequireLowercase", true);
                options.Password.RequireUppercase = Configuration.GetValue("Identity:Password:RequireUppercase", true);
                options.Password.RequireNonAlphanumeric = Configuration.GetValue("Identity:Password:RequireNonAlphanumeric", true);
                options.Password.RequiredLength = Configuration.GetValue("Identity:Password:RequiredLength", 8);
                options.SignIn.RequireConfirmedAccount = false;

            }).AddEntityFrameworkStores<IdentityDb.ApplicationDBContext>()
             .AddSignInManager<SignInManager<LogUser>>().AddUserManager<UserManager<LogUser>>();
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Events.OnRedirectToLogin = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    }

                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
            });

            //////////////////////////////////////////////////

            services.AddControllersWithViews()  // to solve error Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionaryFactory' has been registered.
                .AddViewLocalization()
                .AddJsonOptions(o => o.JsonSerializerOptions
                .ReferenceHandler = ReferenceHandler.Preserve);//to stop the looping in data loading
            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = null);//to stop the looping in data loading
            services.Configure<IfcProcessingOptions>(Configuration.GetSection("IfcProcessing"));
            services.Configure<ViewerOptions>(Configuration.GetSection("Viewer"));
            services.Configure<AdminSeedOptions>(Configuration.GetSection("SeedAdmin"));
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = Configuration.GetValue<long>("IfcProcessing:MaxUploadBytes", 2147483648);
            });
            services.AddScoped<IIfcProcessingService, IfcProcessingService>();
            services.AddCors(options =>
            {
                options.AddPolicy("AngularFrontend", builder =>
                {
                    var origins = Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                        ?? new[] { "http://localhost:4200", "https://localhost:4200" };

                    builder.WithOrigins(origins)
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
            var adminSeed = serviceProvider.GetRequiredService<IOptions<AdminSeedOptions>>().Value;

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

            if (!adminSeed.Enabled || string.IsNullOrWhiteSpace(adminSeed.UserName) || string.IsNullOrWhiteSpace(adminSeed.Password))
            {
                return;
            }

            // find the configured seed admin user
            var _user = await UserManager.FindByNameAsync(adminSeed.UserName);

            // check if the user exists
            var poweruser = new LogUser();
            if (_user == null)
            {
                //Here you could create the super admin who will maintain the web app

                poweruser.FirstName = adminSeed.FirstName;
                poweruser.Email = adminSeed.Email;
                poweruser.UserName = adminSeed.UserName;


                var createPowerUser = await UserManager.CreateAsync(poweruser, adminSeed.Password);
                
                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the role
                    var TestRoleLogUser = await UserManager.AddToRoleAsync(poweruser, "AccountManager");

                }
            }
            else
            {
                if (adminSeed.ResetPasswordOnStartup)
                {
                    var resetToken = await UserManager.GeneratePasswordResetTokenAsync(_user);
                    var resetResult = await UserManager.ResetPasswordAsync(_user, resetToken, adminSeed.Password);
                    if (!resetResult.Succeeded)
                    {
                        var errors = string.Join(" ", resetResult.Errors.Select(error => error.Description));
                        throw new InvalidOperationException($"Unable to reset the configured seed admin password. {errors}");
                    }
                }

                await UserManager.SetLockoutEndDateAsync(_user, null);
                await UserManager.ResetAccessFailedCountAsync(_user);

                if (!await UserManager.IsInRoleAsync(_user, "AccountManager"))
                {
                    await UserManager.AddToRoleAsync(_user, "AccountManager");
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
            app.UseCors("AngularFrontend");
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
                endpoints.MapControllers();
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
