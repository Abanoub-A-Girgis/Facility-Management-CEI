﻿using API.Models;
using Facility_Management_CEI.APIs.Models;
using Facility_Management_CEI.IdentityDb;
using Facility_Management_CEI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Facility_Management_CEI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<LogUser> _userManeger;
        private readonly SignInManager<LogUser> _signInManager;
        public ApplicationDBContext _Context { get; set; }
        public AccountController(UserManager<LogUser> userManger, ApplicationDBContext context,SignInManager<LogUser> singInManager)
        {
            this._userManeger = userManger;
            this._Context = context;
            this._signInManager = singInManager;
        }

        [HttpGet]
        //[Authorize(Roles ="Admin")]
        public IActionResult Register()
        {

            return View();

        }

        public void RegisterAppUser(LogUser user)
        {
            var appuser = new AppUser()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                LogUserId = user.Id,
                Type = API.Enums.UserType.Agent
            };
            _Context.AppUsers.Add(appuser);
            _Context.SaveChanges();
        }




        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
          var newuser = await _userManeger.FindByNameAsync(model.UserName); //must be removed will throw exception
            if(newuser == null)
            {
                var newUser = new LogUser()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.UserName,

                };
                var result=await _userManeger.CreateAsync(newUser, model.PassWord);
                RegisterAppUser(newUser);



                if (result.Succeeded)
                {
                    return View();
                }
                else
                {
                    //need to be handeled 
                    return View();
                }
            }
            else
            {
                ViewData.Add("UserNameIsExist", "Username is already exist");
                return View(model);
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
              //var test = await _userManeger.FindByNameAsync("admin@email");
              //RegisterAppUser(test);

              var result = await _signInManager.PasswordSignInAsync(model.UserName, model.PassWord,false, lockoutOnFailure: false);
              if (result.Succeeded)
              {
                return RedirectToAction("Index", "SensorWarning");

              }
              else
              {
                  
                  return NotFound();
              }
                
            
        }

        public async Task< IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("LogIn");
        }
    }
}
