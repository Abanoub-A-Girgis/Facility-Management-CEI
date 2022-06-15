using API.Moldels_DTOs;
using Facility_Management_CEI.APIs.Models;
using Facility_Management_CEI.IdentityDb;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Facility_Management_CEI.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManger;
        private readonly ApplicationDBContext _context;
        private readonly UserManager<LogUser> _userManeger;

        public RoleController(RoleManager<IdentityRole> roleManager, ApplicationDBContext context,UserManager<LogUser> userManager)
        {
           this._roleManger = roleManager;
           this._context=context;
           this._userManeger=userManager;
        }

        [HttpGet]
        public IActionResult RoleManager()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleManager(string name)
        {
            await _roleManger.CreateAsync(new IdentityRole{ Name= name });
            return View();
        }

        public async Task<IActionResult> Rmove(string roleName)
        {
            var role= await _roleManger.FindByIdAsync(roleName);
            await _roleManger.DeleteAsync(role);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Assign()
        {
            var roles=await _context.Roles.AsNoTracking().ToListAsync();
            var userRoles = new UserRoles(roles);
            return View(userRoles);
        }

        [HttpPost]
        public async Task<IActionResult> Assign(UserRoles ur)
        {
            var user = await _userManeger.FindByNameAsync(ur.UserName);
            if (user != null)
            {
                var role = await _roleManger.FindByNameAsync(ur.Role);
                await _userManeger.AddToRoleAsync(user, ur.Role);
                return RedirectToAction("Assign");
            }
            return RedirectToAction("Register");
        }
    }
}
