using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using WebAPI_HD.Model;
using WebAPI_HD.Repository;
using Response = WebAPI_HD.Model.Response;

namespace WebAPI_HD.Controller
{
    /*[Authorize]*/
    [Route("auth")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private JwtAuthenticationManager _jwtAuth;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        public AccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            JwtAuthenticationManager jwtAuth,
            IConfiguration configuration,
            ApplicationDbContext context
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtAuth = jwtAuth;
            _configuration = configuration;
            _context = context;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username!);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password!))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Id!),
                    new Claim(ClaimTypes.NameIdentifier, user.LastName! + " " + user.FirstName! ),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = _jwtAuth.GenerateAccessToken(authClaims);
                var refreshToken = _jwtAuth.GenerateRefreshToken();
                _ = int.TryParse(_configuration["JWTSettings:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);
                await _userManager.UpdateAsync(user);
                var Token = new JwtSecurityTokenHandler().WriteToken(token);

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7),
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None,
                    MaxAge = TimeSpan.FromDays(7)
                };
                HttpContext.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
                return Ok(new
                {
                    token = Token,
                    RefreshToken = refreshToken,
                    expirationToken = token.ValidTo,
                });
            }
            return Unauthorized(new Response { Status = "Error", Message = "The login detail is incorrect" });
        }
        [AllowAnonymous]
        [HttpPost("register-user")]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            var userExists = await _userManager.FindByNameAsync(model.UserName!);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new ApplicationUser()
            {
                FirstName= model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName,
            };
            var result = await _userManager.CreateAsync(user, model.Password!);
            Console.WriteLine(result);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }
        //[Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpGet("GetUsers")]
        public IActionResult GetAll()
        {
            var users = _userManager.Users.ToList();
            return Ok(users);
        }
        //[Authorize]
        [HttpGet("GetUsers/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
             if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        //[Authorize]
        [HttpGet("currentuser")]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            // Do something with the user information

            return Ok(new
            {
                userId = userId,
                userName = userName,
                userEmail = userEmail
            });
        }
        //[Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin(RegisterRequest model)
        {
            var userExists = await _userManager.FindByNameAsync(model.UserName!);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new ApplicationUser()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName,
            };
            var result = await _userManager.CreateAsync(user, model.Password!);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }
        [AllowAnonymous]
        [HttpPost("register-Superadmin")]
        public async Task<IActionResult> RegisterSuperAdmin(RegisterRequest model)
        {
            var userExists = await _userManager.FindByNameAsync(model.UserName!);
            var superadmin = await _roleManager.FindByNameAsync(UserRoles.SuperAdmin);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });
            if (superadmin != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Role Super-Admin already exist" });

            ApplicationUser user = new ApplicationUser()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName,
            };
            var result = await _userManager.CreateAsync(user, model.Password!);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.SuperAdmin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.SuperAdmin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (await _roleManager.RoleExistsAsync(UserRoles.SuperAdmin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.SuperAdmin);
            }
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }
        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
        {
            if (tokenModel is null)
            {
                return BadRequest("Invalid client request");
            }
            string? accessToken = tokenModel.AccessToken;
            var refreshToken = Request.Cookies["refreshToken"];

            var principal = _jwtAuth.GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            { 
                return BadRequest("Invalid access token");
            }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            string username = principal.Identity.Name;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            var user = await _userManager.FindByIdAsync(username!);
            if (!user!.RefreshToken!.Equals(refreshToken) || user == null)
            {
                return Unauthorized(new Response { Status = "Error", Message = "Invaild RefreshToken" });
            }else if (user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return Unauthorized(new Response { Status = "Error", Message = "Token expired" });
            }
            var newAccessToken = _jwtAuth.GenerateAccessToken(principal.Claims.ToList());
            var newRefreshToken = _jwtAuth.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7),
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None,
                MaxAge = TimeSpan.FromDays(7)
            };
            HttpContext.Response.Cookies.Append("refreshToken", newRefreshToken, cookieOptions);
            return new ObjectResult(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                refreshToken = newRefreshToken,
            });
        }
        //[Authorize]
        [HttpPost]
        [Route("revoke/{username}")]
        public async Task<IActionResult> Revoke(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return BadRequest("Invalid user name");

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);

            return NoContent();

        }
        //[Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpPost]
        [Route("revoke-all")]
        public async Task<IActionResult> RevokeAll()
        {
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);
            }

            return NoContent();
        }
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> Update(ApplicationUser model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }
            user.Email = model.Email;
            user.LastName= model.LastName;
            user.FirstName= model.FirstName;
            user.PhoneNumber= model.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { message = "User updated successfully" });
            }
            return NotFound();
        }
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userManager.DeleteAsync(user!);
            if (result.Succeeded)
            {
                return Ok(new { message = "User deleted successfully" });
            }
            return NotFound();
        }
    }
}
