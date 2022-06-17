using API.DB;
using API.Enums;
using Facility_Management_CEI.APIs.Models;
using Facility_Management_CEI.IdentityDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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


        //------------------------


        public void ConfigureServices(IServiceCollection services)
        {


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

            services.AddControllersWithViews()  // to solve erro Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionaryFactory' has been registered.
                .AddJsonOptions(o => o.JsonSerializerOptions
                .ReferenceHandler = ReferenceHandler.Preserve);//to stop the looping in data loading
            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve);//to stop the looping in data loading
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

            string[] roleNames = Enum.GetNames(typeof(UserType)); /*{ "SystemAdmin", "Owner", "Manager", "Supervisor","Inspector","Agent" };*/


           


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


                var createPowerUser = await UserManager.CreateAsync(poweruser, adminPassword);
                
                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the role
                    var TestRoleLogUser = await UserManager.AddToRoleAsync(poweruser, "SystemAdmin");

                }
            }
        }
        #region code to copy from
        //-----------------------------------
        //--------------------------------

        //private void createRolesandUsers()
        //{
        //    IdentityDb.ApplicationDBContext context = new ApplicationDBContext();

        //    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        //    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


        //    // In Startup iam creating first Admin Role and creating a default Admin User     
        //    if (!roleManager.RoleExists("Admin"))
        //    {

        //        // first we create Admin rool    
        //        var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
        //        role.Name = "Admin";
        //        roleManager.Create(role);

        //        //Here we create a Admin super user who will maintain the website                   

        //        var user = new ApplicationUser();
        //        user.UserName = "shanu";
        //        user.Email = "syedshanumcain@gmail.com";


                string adminPassword = "Admin!123";
                var createPowerUser = await UserManager.CreateAsync(poweruser, adminPassword);
                if (createPowerUser.Succeeded)
                {


                   //here we tie the new user to the role
                   await UserManager.AddToRoleAsync(poweruser, "SystemAdmin");

                    
                }


            }

            //-------------------------
            
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {

            CreateRoles(serviceProvider).Wait();


        //-------------------------
        #endregion
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider  serviceProvider)
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


                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();



            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

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