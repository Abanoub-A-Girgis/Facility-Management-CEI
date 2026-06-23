using Facility_Management_CEI.ApiLayer.Dtos;
using Facility_Management_CEI.APIs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Facility_Management_CEI.ApiLayer.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<LogUser> _signInManager;
        private readonly UserManager<LogUser> _userManager;

        public AuthController(SignInManager<LogUser> signInManager, UserManager<LogUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var result = await _signInManager.PasswordSignInAsync(
                request.UserName,
                request.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return Unauthorized(ApiResponse<object>.Fail("Invalid username or password."));
            }

            var user = await _userManager.FindByNameAsync(request.UserName);
            return Ok(ApiResponse<AuthUserDto>.Ok(await ToDto(user)));
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(ApiResponse<object>.Fail("User is not authenticated."));
            }

            return Ok(ApiResponse<AuthUserDto>.Ok(await ToDto(user)));
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(ApiResponse<object>.Ok(null, "Signed out."));
        }

        private async Task<AuthUserDto> ToDto(LogUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var displayName = string.Join(" ", new[] { user.FirstName, user.LastName }.Where(part => !string.IsNullOrWhiteSpace(part)));

            return new AuthUserDto
            {
                Id = user.Id,
                AppUserId = user.AppUser?.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DisplayName = string.IsNullOrWhiteSpace(displayName) ? user.UserName : displayName,
                Roles = roles.ToList()
            };
        }
    }
}
