using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenRepository _tokenRepository;
        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            _userManager = userManager;
            _tokenRepository = tokenRepository;
        }


        //Register

        [HttpPost]
        [Route("Register")]

        public async Task<IActionResult> Register([FromBody] RegisterRequestDto RegisterDto)
        {

            var identityUser = new IdentityUser
            {
                UserName = RegisterDto.Username,
                Email = RegisterDto.Username
            };

            var identityResult = await _userManager.CreateAsync(identityUser , RegisterDto.Password);

            if (identityResult.Succeeded)
            {
                return Ok("User was registered! Please login.");
            }

            return BadRequest("Something went wrong");

        }


        // login
        [HttpPost]
        [Route("Login")]

        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDto.Username);

            if(user != null)
            {
                var checkPasswordResult = await _userManager.CheckPasswordAsync(user,loginRequestDto.Password);

                if (checkPasswordResult)
                {
                    // Get Roles for this user

                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles != null)
                    {
                        // Create Token

                        var jwtToken = _tokenRepository.CreateJWTToken(user, roles.ToList());

                        var response = new LoginResponseDto
                        {
                            JwtToken = jwtToken
                        };

                        return Ok(response);
                    }
                }
            }

            return BadRequest("Username or password incorrect");



        }

    }
}
