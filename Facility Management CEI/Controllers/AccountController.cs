using API.Models;
using Facility_Management_CEI.APIs.Models;
using Facility_Management_CEI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Facility_Management_CEI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<LogUser> _userMager;
        public AccountController(UserManager<LogUser> userManger)
        {
            this._userMager=userManger;
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel ouruser)
        {
           var newuser = await _userMager.FindByNameAsync(ouruser.UserName); 
            if(ouruser == null)
            {
                var newUser = new LogUser()
                {
                    UserName = ouruser.UserName,
                    //PasswordHash=model.PassWord
                };
                var result=await _userMager.CreateAsync(newUser, ouruser.PassWord);
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
                return View(ouruser);
            }
        }

        public IActionResult LogIn(LogInViewModel model)
        {
            return View();
        }

        public IActionResult LogOut()
        {
            return View();
        }
    }
}
