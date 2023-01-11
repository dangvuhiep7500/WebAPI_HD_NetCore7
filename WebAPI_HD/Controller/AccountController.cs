using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebAPI_HD.Model;
using WebAPI_HD.Repository;
using Response = WebAPI_HD.Model.Response;

namespace WebAPI_HD.Controller
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        /*private IUserService _userService;*/
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private JwtAuthenticationManager _jwtAuth;

        public AccountController(
             UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            JwtAuthenticationManager jwtAuth)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtAuth = jwtAuth;
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
                    new Claim(ClaimTypes.Name, user.UserName!),
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
            return Ok(new Response { Status = "Error", Message = "Username or password is incorrect" });
        }
        [AllowAnonymous]
        [HttpPost("register")]
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
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userManager.Users.ToList();
            return Ok(users);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
             if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [Authorize(Roles = UserRoles.SuperAdmin)]
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
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("update")]
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
        [HttpDelete("{id}")]
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
