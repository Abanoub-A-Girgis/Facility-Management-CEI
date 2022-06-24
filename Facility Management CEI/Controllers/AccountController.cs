using API.Enums;
using API.Models;
using Facility_Management_CEI.APIs.Models;
using Facility_Management_CEI.IdentityDb;
using Facility_Management_CEI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Facility_Management_CEI.Controllers
{
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManger;
        private readonly UserManager<LogUser> _userManeger;
        private readonly SignInManager<LogUser> _signInManager;
        public ApplicationDBContext _Context { get; set; }
        public AccountController(UserManager<LogUser> userManger, ApplicationDBContext context, RoleManager<IdentityRole> roleManger, SignInManager<LogUser> singInManager)
        {
            this._userManeger = userManger;
            this._Context = context;
            this._signInManager = singInManager;
            this._roleManger = roleManger;
        }



        [HttpGet]
        [Authorize(Roles = "SystemAdmin")]
        public IActionResult Register()
        {
            var user = new RegisterViewModel();//to send a model that has a list of roles

            return View(user);
        }


        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var newuser = await _userManeger.FindByNameAsync(model.UserName); //must be removed will throw exception
            if (newuser == null)
            {
                var newUser = new LogUser()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.UserName,

                };
                var result = await _userManeger.CreateAsync(newUser, model.PassWord);

                if (result.Succeeded)
                {
                    //here we tie the new user to the role
                    var TestRoleLogUser = await _userManeger.AddToRoleAsync(newUser, model.Role);
                    if (TestRoleLogUser.Succeeded)
                    {
                        //UserType AppUserType;
                        var testParse = Enum.TryParse(model.Role, out UserType AppUserType);
                        var appuser = new AppUser()
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            LogUserId = newUser.Id,
                            Type = AppUserType/*Enum.TryParse("Active", out StatusEnum myStatus)*/,//need yo make the role = the tupe in the RegisterViewModel
                            BuildingId = model.BuildingId,
                            SuperId = model.SuperId
                        };
                        _Context.AppUsers.Add(appuser);
                        _Context.SaveChanges();

                    }
                    var user = new RegisterViewModel();//to send a model that has a list of roles
                    return View(user);
                }
                else
                {
                    var user = new RegisterViewModel();//to send a model that has a list of roles
                    //need to be handeled 
                    return View(user);
                }
            }
            else
            {
                var user = new RegisterViewModel();
                ViewData.Add("UserNameIsExist", "Username is already exist");
                return View(user);
            }
        }
        [HttpGet]
        public /*async Task<*/IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LogInViewModel model)
        {
           
            var UsersList = await _Context.AppUsers.ToListAsync();
            if (UsersList.Count == 0) 
            {
                var Admin = await _userManeger.FindByNameAsync("admin@email");
                var appuser = new AppUser()

                {
                    FirstName = Admin.FirstName,
                    LastName = Admin.LastName,
                    LogUserId = Admin.Id,
                    Type = API.Enums.UserType.SystemAdmin
                };
                _Context.AppUsers.Add(appuser);
                _Context.SaveChanges();

            }
            

            {
                var SignedInLogUser = await _Context.LogUsers.FirstOrDefaultAsync(i => i.UserName == model.UserName);
                if (SignedInLogUser != null)
                {
                    var SignedInAppUser = SignedInLogUser.AppUser;
                    if (SignedInAppUser != null)
                    {
                        var RoleOfAppUser = SignedInAppUser.Type.ToString();
                        var Flag = await _userManeger.IsInRoleAsync(SignedInLogUser, RoleOfAppUser);
                        if (!Flag)
                        {
                            var ttt = await _userManeger.RemoveFromRolesAsync(SignedInLogUser, Enum.GetNames(typeof(UserType)));
                            await _userManeger.AddToRoleAsync(SignedInLogUser, RoleOfAppUser);

                        }
                    }
                }

                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.PassWord, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("HomePage", "HomePage");
                }
                else
                {
                    return View();
                }


            }
        }

            public async Task<IActionResult> LogOut()
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("LogIn");
            }
        
    }

}

