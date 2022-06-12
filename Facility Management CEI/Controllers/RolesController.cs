using Facility_Management_CEI.IdentityDb;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Facility_Management_CEI.Controllers
{
    public class RolesController : Controller
    {

        private readonly RoleManager<IdentityRole> _roleManger;
        private readonly ApplicationDBContext _context;
        public RolesController(RoleManager<IdentityRole> roleManager, ApplicationDBContext context)
        {
           this._roleManger = roleManager;
           this._context=context;
        }
        public async Task<IActionResult> Index()
        {
            var roles=await _context.Roles.ToListAsync();
            return View(roles);
        }


        [HttpPost]
        public IActionResult RoleManager(string roleName)
        {
            _roleManger.CreateAsync(new IdentityRole { Name = roleName });
            return View();
        }
        public async Task<IActionResult> Rmove(string roleId)
        {
            var role= await _roleManger.FindByIdAsync(roleId);
            await _roleManger.DeleteAsync(role);
            return View();
        }
    }
}
