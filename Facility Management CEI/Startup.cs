using API.DB;
using Facility_Management_CEI.APIs.Models;
using Facility_Management_CEI.IdentityDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<IdentityDb.ApplicationDBContext>(options =>
            options.UseSqlServer(
            Configuration.GetConnectionString("ProjectDataBase")));
            services.AddControllersWithViews();

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
            services.AddControllers()
                .AddJsonOptions(o => o.JsonSerializerOptions
                .ReferenceHandler = ReferenceHandler.Preserve);//to stop the looping in data loading
            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve);//to stop the looping in data loading
            //services.AddDbContext<API.DB.ApplicationDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ProjectDataBase")));
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
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Register}/{id?}");
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