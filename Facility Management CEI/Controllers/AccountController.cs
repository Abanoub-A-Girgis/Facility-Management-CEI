using API.Models;
using Facility_Management_CEI.APIs.Models;
using Facility_Management_CEI.IdentityDb;
using Facility_Management_CEI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Facility_Management_CEI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<LogUser> _userManeger;
        public ApplicationDBContext _Context { get; set; }
        public AccountController(UserManager<LogUser> userManger, ApplicationDBContext context)
        {
            this._userManeger = userManger;
            _Context = context;
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

                var appuser = new AppUser()
                {
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    LogUserId = newUser.Id,
                    Type = 0
                };
                _Context.AppUsers.Add(appuser);
                _Context.SaveChanges();

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


        public IActionResult Register()
        {

            return View();

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
