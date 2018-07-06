using System;
using System.Linq;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using TodoApi.Data;
using TodoApi.Data.Models;
using TodoApi.Web.Models;

namespace TodoApi.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private TodoContext _context;
        private ILogger<AuthController> _logger;
        private UserManager<TodoUser> _userMgr;
        private IPasswordHasher<TodoUser> _hasher;
        private IConfiguration _config;

        public AuthController(
            TodoContext context,
            SignInManager<TodoUser> signInMgr,
            UserManager<TodoUser> userMgr,
            IPasswordHasher<TodoUser> hasher,
            ILogger<AuthController> logger,
            IConfiguration config)
        {
            _context = context;
            _logger = logger;
            _userMgr = userMgr;
            _hasher = hasher;
            _config = config;
        }

        private async Task<IActionResult> ReturnToken(TodoUser user)
        {
            var userClaims = await _userMgr.GetClaimsAsync(user);

            var claims = new[]
            {
                              new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                              new Claim(JwtRegisteredClaimNames.Email, user.Email)
                    }.Union(userClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
              issuer: _config["Tokens:Issuer"],
              audience: _config["Tokens:Audience"],
              claims: claims,
              expires: DateTime.UtcNow.AddMinutes(15),
              signingCredentials: creds
              );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] CredentialModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(412);
                }

                var user = new TodoUser() { Email = model.Email, UserName = model.Email };
                var result = await _userMgr.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return await this.ReturnToken(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown while creating JWT: {ex}");
            }

            return BadRequest("Failed to generate token");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] CredentialModel model)
        {
            try
            {
                var user = await _userMgr.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (_hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Success)
                    {
                        return await this.ReturnToken(user);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown while creating JWT: {ex}");
            }

            return BadRequest("Failed to generate token");
        }

        //readonly UserManager<IdentityUser> userManager;
        //readonly SignInManager<IdentityUser> signInManager;

        //public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        //{
        //    this.userManager = userManager;
        //    this.signInManager = signInManager;
        //}

        //[HttpPost]
        //public async Task<IActionResult> Register([FromBody] Credentials credentials)
        //{
        //    var user = new IdentityUser { UserName = credentials.Email, Email = credentials.Email };

        //    var result = await userManager.CreateAsync(user, credentials.Password);

        //    if (!result.Succeeded)
        //        return BadRequest(result.Errors);

        //    await signInManager.SignInAsync(user, isPersistent: false);

        //    return Ok(CreateToken(user));
        //}

        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] Credentials credentials)
        //{
        //    var result = await signInManager.PasswordSignInAsync(credentials.Email, credentials.Password, false, false);

        //    if (!result.Succeeded)
        //        return BadRequest();

        //    var user = await userManager.FindByEmailAsync(credentials.Email);

        //    return Ok(CreateToken(user));
        //}

        //string CreateToken(IdentityUser user)
        //{
        //    var claims = new Claim[]
        //   {
        //        new Claim(JwtRegisteredClaimNames.Sub, user.Id)
        //   };

        //    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is the secret phrase"));
        //    var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        //    var jwt = new JwtSecurityToken(signingCredentials: signingCredentials, claims: claims);
        //    return new JwtSecurityTokenHandler().WriteToken(jwt);
        //}
    }
}