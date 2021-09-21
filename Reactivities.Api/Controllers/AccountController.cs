using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Reactivities.Api.DTOs;
using Reactivities.Api.Services;
using Reactivities.Domain;
using System.Threading.Tasks;

namespace Reactivities.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public AccountController(UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager, 
            TokenService tokenService)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            TokenService = tokenService;
        }

        public UserManager<AppUser> UserManager { get; }
        public SignInManager<AppUser> SignInManager { get; }
        public TokenService TokenService { get; }


        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto dto)
        {
            var user = await UserManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return Unauthorized();

            var result = await SignInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            if (result.Succeeded)
            {
                return new UserDto
                {
                    DisplayName = user.DisplayName,
                    Username = user.UserName,
                    Image = null,
                    Token = TokenService.CreateToken(user)
                };
            }

            return Unauthorized();
        }
    }
}
