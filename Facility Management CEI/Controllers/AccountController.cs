using API.Enums;
using API.Models;
using Facility_Management_CEI.APIs.Models;
using Facility_Management_CEI.IdentityDb;
using Facility_Management_CEI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
        [Authorize(Roles = "AccountManager")]
        public IActionResult Register()
        {
            try
            {
                var user = new RegisterViewModel();//to send a model that has a list of roles
                ViewData["SuperId"] = new SelectList(_Context.AppUsers.Where(user => user.Type != UserType.Agent && user.Type != UserType.AccountManager).Select(s => new { FullText = s.Id + ": " + s.FirstName + " " + s.LastName, Id = s.Id }), "Id", "FullText");

                //ViewData["SuperId"] = new SelectList(_Context.AppUsers.Where(user => user.Type != UserType.Agent), "Id", "Id");//this is made to lsit down all the ids that can be used as a super id
                return View(user);
            }
            catch (Exception ex)
            {
                ErrorMessage.Message = ex.Message.ToString();
                return RedirectToAction("ErrorGeneric", "ErrorPages");
            }

        }

        [HttpPost]
        [Authorize(Roles = "AccountManager")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
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
                            var SuperAppUser = await _Context.AppUsers.FirstOrDefaultAsync(i => i.Id == model.SuperId);//to add the same biuldingId of the Super
                            var testParse = Enum.TryParse(model.Role, out UserType AppUserType);

                            var appuser = new AppUser()
                            {
                                FirstName = model.FirstName,
                                LastName = model.LastName,
                                LogUserId = newUser.Id,
                                Type = AppUserType/*Enum.TryParse("Active", out StatusEnum myStatus)*/,//need yo make the role = the tupe in the RegisterViewModel
                                BuildingId = SuperAppUser?.BuildingId,
                                SuperId = model.SuperId
                            };

                            _Context.AppUsers.Add(appuser);
                            _Context.SaveChanges();

                        }
                        var user = new RegisterViewModel();//to send a model that has a list of roles
                        ViewData["SuperId"] = new SelectList(_Context.AppUsers.Where(user => user.Type != UserType.Agent && user.Type != UserType.AccountManager).Select(s => new { FullText = s.Id + ": " + s.FirstName + " " + s.LastName, Id = s.Id }), "Id", "FullText");
                        ViewBag.English = "Registration has been successfully completed";
                        ViewBag.Arabic = "تم التسجيل بنجاح";
                        ViewBag.RegistrationStatus = true;
                        return View(user);
                        //return NoContent();
                        //return RedirectToAction("Register", "AccountController");
                    }
                    else
                    {
                        var user = new RegisterViewModel();//to send a model that has a list of roles
                        ViewData["SuperId"] = new SelectList(_Context.AppUsers.Where(user => user.Type != UserType.Agent && user.Type != UserType.AccountManager).Select(s => new { FullText = s.Id + ": " + s.FirstName + " " + s.LastName, Id = s.Id }), "Id", "FullText");                                   //need to be handeled 
                        ViewBag.English = "Username or password is incorrect, please try again";
                        ViewBag.Arabic = "خطأ في اسم المستخدم أو كلمة السر، حاول مرة اخرى";
                        ViewBag.RegistrationStatus = false;
                        return View(user);
                        //return NoContent();
                    }
                }
                else
                {
                    var user = new RegisterViewModel();
                    ViewData.Add("UserNameIsExist", "Username is already exist");
                    ViewBag.English = "Specified username already exists";
                    ViewBag.Arabic = "اسم المستخدم موجود مسبقا";
                    ViewBag.RegistrationStatus = false;
                    ViewData["SuperId"] = new SelectList(_Context.AppUsers.Where(user => user.Type != UserType.Agent && user.Type != UserType.AccountManager).Select(s => new { FullText = s.Id + ": " + s.FirstName + " " + s.LastName, Id = s.Id }), "Id", "FullText");
                    return View(user);
                    //return NoContent();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Message = ex.Message.ToString();
                return RedirectToAction("ErrorGeneric", "ErrorPages");
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
            try
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
                        Type = API.Enums.UserType.AccountManager
                    };
                    _Context.AppUsers.Add(appuser);
                    _Context.SaveChanges();

                }
                {
                    var SignedInLogUser = await _Context.LogUsers.FirstOrDefaultAsync(i => i.UserName == model.UserName);//need to be tested with ramy code
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
                        ViewBag.English = "Username or password is incorrect, please try again";
                        ViewBag.Arabic = "خطأ في اسم المستخدم أو كلمة السر، حاول مرة اخرى";
                        return View();
                    }


                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Message = ex.Message.ToString();
                return RedirectToAction("ErrorGeneric", "ErrorPages");
            }

           
        }

         public async Task<IActionResult> LogOut()
         {
            try
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("LogIn");
            }
            catch (Exception ex)
            {
                ErrorMessage.Message = ex.Message.ToString();
                return RedirectToAction("ErrorGeneric", "ErrorPages");
            }

         }
        //MobileApp Code
        [HttpPost]
        public List<LogUser> MobileUsers() 
        {
            var UsersList =  _Context.LogUsers.ToList();
            return UsersList;
        }

        public List<AppUser> LoginToMobApp()
        {
            return _Context.AppUsers.ToList();
        }
        [HttpGet]
        public IActionResult LoginTomobApp1()
        {
            var appusers = _Context.AppUsers.Where(u => u.Type == UserType.Agent).Select(u => new {id = u.Id, u.FirstName, u.LastName, u.SuperId, u.ProfilePicturePath});
            return Ok(appusers);
        }
    }

}

