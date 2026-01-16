using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WineLabelMakerBE.Models.DTOs.Identity.Request;
using WineLabelMakerBE.Models.DTOs.Identity.Response;
using WineLabelMakerBE.Models.Entity;

namespace WineLabelMakerBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AspNetUserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AspNetUserController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        //REGISTER
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApplicationUser user = new ApplicationUser()
                    {
                        UserName = registerRequestDto.Email,
                        Email = registerRequestDto.Email,
                        Name = registerRequestDto.Name,
                        Surname = registerRequestDto.Surname,
                        PhoneNumber = registerRequestDto.PhoneNumber,
                        CreatedAt = DateTime.Now,
                        Birthday = registerRequestDto.Birthday,
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


        // POST LOGIN 
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            try
            {
                //Controllo se l'utente esiste
                ApplicationUser user = await _userManager.FindByNameAsync(loginRequestDto.Username);
                if (user is null)
                {
                    return BadRequest();
                }

                //Controllo login 
                var result = await _signInManager.PasswordSignInAsync(user, loginRequestDto.Password, false, false);

                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }

                //Ruoli dell'utente
                List<string> roles = (await this._userManager.GetRolesAsync(user)).ToList();

                //Creazione claims
                List<Claim> userClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                };
                foreach (string roleName in roles)
                {
                    userClaims.Add(new Claim(ClaimTypes.Role, roleName));
                }

                //Generazione token JWT
                var key = System.Text.Encoding.UTF8.GetBytes("60a2add30a059f078613cc20c9fa26664b58bf7c286737a23c23f1d18b79518c0a82955c");
                SigningCredentials cred = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256);

                var tokenExpiration = DateTime.Now.AddMinutes(30);

                JwtSecurityToken jwt = new JwtSecurityToken(
                    "https://", //Issuer
                    "https://", //Audience
                    claims: userClaims,
                    expires: tokenExpiration,
                    signingCredentials: cred
                    );

                string token = new JwtSecurityTokenHandler().WriteToken(jwt);

                return Ok(new LoginResponseDto()
                {
                    Token = token,
                    Expiration = tokenExpiration

                });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }


        }
    }
}
