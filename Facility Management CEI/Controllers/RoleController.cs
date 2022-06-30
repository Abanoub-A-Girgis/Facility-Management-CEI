using API.Moldels_DTOs;
using Facility_Management_CEI.APIs.Models;
using Facility_Management_CEI.IdentityDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Principal;
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
        [Authorize(Roles = "SystemAdmin")]
        public IActionResult RoleManager()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> RoleManager(string name)
        {
            try
            {
                await _roleManger.CreateAsync(new IdentityRole { Name = name });
                return View();
            }
            catch (Exception ex)
            {
                ErrorMessage.Message = ex.Message.ToString();
                return RedirectToAction("Error404", "ErrorPages");
            }

        }

        public async Task<IActionResult> Rmove(string roleName)
        {
            try
            {
                var role = await _roleManger.FindByIdAsync(roleName);
                await _roleManger.DeleteAsync(role);
                return View();
            }
            catch (Exception ex)
            {
                ErrorMessage.Message = ex.Message.ToString();
                return RedirectToAction("Error404", "ErrorPages");
            }

        }

        [HttpGet]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> Assign()
        {
            try
            {
                var roles = await _context.Roles.AsNoTracking().ToListAsync();
                var userRoles = new UserRoles(roles);
                return View(userRoles);
            }
            catch (Exception ex)
            {
                ErrorMessage.Message = ex.Message.ToString();
                return RedirectToAction("Error404", "ErrorPages");
            }

        }

        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> Assign(UserRoles ur)
        {
            try
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
            catch (Exception ex)
            {
                ErrorMessage.Message = ex.Message.ToString();
                return RedirectToAction("Error404", "ErrorPages");
            }

        }

    }
    public static class PrincipalExtensions
    {
        //public static bool IsInAllRoles(this IPrincipal principal, params string[] roles)
        //{
        //    return roles.All(r => principal.IsInRole(r));
        //}

        public static bool IsInAnyRoles(this IPrincipal principal, params string[] roles)
        {
            return roles.Any(r => principal.IsInRole(r));
        }
    }

}
