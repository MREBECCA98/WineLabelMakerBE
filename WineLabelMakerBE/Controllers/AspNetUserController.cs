using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WineLabelMakerBE.Models.DTOs.Identity.Request;
using WineLabelMakerBE.Models.DTOs.Identity.Response;
using WineLabelMakerBE.Models.Entity;

//AspNetUserController is created for user registration and login
//When the user logs in, a JWT token is generated that the client uses to authenticate
//to the application's secure endpoints

namespace WineLabelMakerBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AspNetUserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AspNetUserController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        //REGISTER
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    ApplicationUser user = new ApplicationUser()
                    {
                        UserName = registerRequestDto.Email,
                        Email = registerRequestDto.Email,
                        CompanyName = registerRequestDto.CompanyName,
                        Name = registerRequestDto.Name,
                        Surname = registerRequestDto.Surname,
                        PhoneNumber = registerRequestDto.PhoneNumber,
                        CreatedAt = DateTime.UtcNow,

                        Id = Guid.NewGuid().ToString(),
                        IsDeleted = false,
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                    };
                    IdentityResult result = await _userManager.CreateAsync(user, registerRequestDto.Password);
                    if (result.Succeeded)
                    {
                        var roleExists = await _roleManager.RoleExistsAsync("User");
                        if (!roleExists)
                        {
                            await _roleManager.CreateAsync(new IdentityRole("User"));
                        }
                        await _userManager.AddToRoleAsync(user, "User");
                        return Ok();
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return BadRequest();
        }


        //POST LOGIN 
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            try
            {
                //Check if the user exists
                ApplicationUser user = await _userManager.FindByNameAsync(loginRequestDto.Username);
                if (user is null)
                {
                    return BadRequest();
                }

                //Login control
                var result = await _signInManager.PasswordSignInAsync(user, loginRequestDto.Password, false, false);

                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }

                //User roles
                List<string> roles = (await this._userManager.GetRolesAsync(user)).ToList();

                //Creation of claims
                List<Claim> userClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("Name", user.Name ?? ""),
                    new Claim("Surname", user.Surname ?? ""),
                    new Claim("CompanyName", user.CompanyName ?? "")
                };
                foreach (string roleName in roles)
                {
                    userClaims.Add(new Claim(ClaimTypes.Role, roleName));
                }

                //JWT Token Generation
                var key = System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:SecurityKey"]);
                SigningCredentials cred = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256);

                var tokenExpiration = DateTime.UtcNow.AddHours(4);

                JwtSecurityToken jwt = new JwtSecurityToken(
                     issuer: _configuration["Jwt:Issuer"],
                     audience: _configuration["Jwt:Audience"],
                     claims: userClaims,
                     expires: tokenExpiration,
                     signingCredentials: cred);


                string token = new JwtSecurityTokenHandler().WriteToken(jwt);

                return Ok(new LoginResponseDto()
                {
                    Token = token,
                    Expiration = tokenExpiration,
                    Role = roles.FirstOrDefault()

                });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
