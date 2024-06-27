using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reddit.Models;
using Reddit.Services;

namespace Reddit.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly TokenManager _tokenManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<User> userManager,  TokenManager tokenManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _tokenManager = tokenManager;
            _context = context;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(Register register)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userManager.CreateAsync
            (
                new User { UserName = register.UserName, Email = register.Email, RefreshToken = _tokenManager.CreateRefreshToken(), RefreshTokenExpiration = DateTime.Now.AddDays(1) },
                register.Password!
            );

            if (result.Succeeded)
            {
                register.Password = "";
                return CreatedAtAction(nameof(Register), new { email = register.Email }, register);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return BadRequest(ModelState);
        }


        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<AuthRes>> Login([FromBody] AuthReq request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(request.Email!);
            if (user == null)
            {
                return BadRequest("Incorrect credentials");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password!);
            if (!passwordValid)
            {
                return BadRequest("Incorrect credentials");
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (existingUser is null)
            {
                return Unauthorized();
            }

            var token = _tokenManager.CreateToken(existingUser);
            await _context.SaveChangesAsync();

            return Ok
            (
                new AuthRes
                {
                    UserName = existingUser.UserName,
                    Email = existingUser.Email,
                    Token = token,
                    RefreshToken = existingUser.RefreshToken,
                }
            );
        }

        [HttpPost("Refresh_Token")]
        public async Task<IActionResult> RefreshToken([FromBody] string RefreshToken)
        {
            if (RefreshToken == null)
                return BadRequest("Bad Refresh Token");

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshToken == RefreshToken);
            if (user == null || user.RefreshTokenExpiration <= DateTime.Now)
                return BadRequest("Unknown or expired Refresh Token");

            var newToken = _tokenManager.CreateRefreshToken();
            user.RefreshToken = newToken;
            user.RefreshTokenExpiration = DateTime.UtcNow.AddDays(1);
            await _userManager.UpdateAsync(user);

            return Ok(new { accessToken = _tokenManager.CreateToken(user), refreshToken = newToken} );
        }
    }
}
