using AutoMapper;
using Azure;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI_HD.Model;
using WebAPI_HD.Repository;
using Response = WebAPI_HD.Model.Response;

namespace WebAPI_HD.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        /*private IUserService _userService;*/
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private JwtAuthenticationManager _jwtAuth;
        private readonly JWTSettings _jwtsettings;

        public AccountController(
             UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            JwtAuthenticationManager jwtAuth,
            IOptions<JWTSettings> jwtsettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtsettings = jwtsettings.Value;
            _jwtAuth= jwtAuth;
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null || BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = _jwtAuth.GenerateAccessToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            
            return Unauthorized();

        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, BCrypt.Net.BCrypt.HashPassword(model.Password));
            Console.WriteLine(result);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }
        /*
         [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(RegisterRequest model)
        {
            _userService.Register(model);
            return Ok(new { message = "Registration successful" });
        }
                [HttpGet]
                public IActionResult GetAll()
                {
                    var users = _userService.GetAll();
                    return Ok(users);
                }

                [HttpGet("{id}")]
                public IActionResult GetById(int id)
                {
                    var user = _userService.GetById(id);
                    return Ok(user);
                }

                [HttpPut("{id}")]
                public IActionResult Update(int id, UpdateRequest model)
                {
                    _userService.Update(id, model);
                    return Ok(new { message = "User updated successfully" });
                }

                [HttpDelete("{id}")]
                public  IActionResult Delete(int id)
                {
                    _userService.Delete(id);
                    return Ok(new { message = "User deleted successfully" });
                }*/
    }
}
