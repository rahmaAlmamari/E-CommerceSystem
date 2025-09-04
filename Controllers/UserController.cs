using AutoMapper;
using E_CommerceSystem;//to see JwtSettings ...
using E_CommerceSystem.Models;
using E_CommerceSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; //for CookieOptions ...
using Microsoft.Extensions.Options;//for IOptions<JwtSettings> ...
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using E_CommerceSystem.Auth;

namespace E_CommerceSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[Controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwt;          

        public UserController(
            IUserService userService,
            IAuthService authService,
            IMapper mapper,
            IOptions<JwtSettings> jwt                 
        )
        {
            _userService = userService;
            _authService = authService;
            _mapper = mapper;
            _jwt = jwt.Value;                          
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserDTO InputUser)
        {
            try
            {
                if (InputUser == null)
                    return BadRequest("User data is required");
                //AutoMapper to map UserDTO to User ...
                var user = _mapper.Map<User>(InputUser);
                user.CreatedAt = DateTime.UtcNow;
                // Set default role if not provided ...
                var normalizedRole = string.IsNullOrWhiteSpace(user.Role)
                 ? Roles.Customer
                 : user.Role.Trim().ToLowerInvariant();

                if (normalizedRole != Roles.Customer && normalizedRole != Roles.Manager && normalizedRole != Roles.Admin)
                    return BadRequest("Invalid role. Allowed: admin, manager, customer.");

                user.Role = normalizedRole;

                // === Added: hash the incoming plain-text password from DTO before saving ===
                // Make sure your UserDTO includes a Password field coming from the request.
                user.Password = BCrypt.Net.BCrypt.HashPassword(InputUser.Password, workFactor: 12);

                _userService.AddUser(user);
                return Ok(user);
            }
            catch (Exception ex)
            {
                // Return a generic error response ...
                return StatusCode(500, $"An error occurred while adding the user. {ex.Message} ");
            }
        }


        //to login and get JWT access + refresh tokens ...
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var (access, refresh) = await _authService.SignInAsync(dto.UsernameOrEmail, dto.Password, GetIp());

                SetCookie("access_token", access, minutes: _jwt.AccessTokenMinutes); // matches JwtSettings via TokenService ...
                SetCookie("refresh_token", refresh.Token, days: _jwt.RefreshTokenDays);

                // Also return token in body for Swagger testing ...
                return Ok(new { token = access });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                // Return a generic error response ...
                return StatusCode(500, $"An error occurred while login. {ex.Message}");
            }
        }

        //to refresh with rotation ...
        [AllowAnonymous]
        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh(
            [FromHeader(Name = "X-Refresh-Token")] string? headerRefreshToken = null
        )
        {
            try
            {
                var rt = Request.Cookies["refresh_token"] ?? headerRefreshToken;//cookie first, then header
                if (string.IsNullOrEmpty(rt)) return Unauthorized("Missing refresh token.");

                var (access, newRefresh) = await _authService.RefreshAsync(rt, GetIp());

                SetCookie("access_token", access, minutes: _jwt.AccessTokenMinutes);
                SetCookie("refresh_token", newRefresh.Token, days: _jwt.RefreshTokenDays);

                return Ok(new { token = access });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while refreshing token. {ex.Message}");
            }
        }


        //revoke current refresh token + clear cookies ...
        [AllowAnonymous]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var rt = Request.Cookies["refresh_token"];
                if (!string.IsNullOrEmpty(rt))
                    await _authService.RevokeAsync(rt, GetIp());

                Response.Cookies.Delete("access_token");
                Response.Cookies.Delete("refresh_token");

                return Ok(new { message = "Logged out." });
            }
            catch (Exception ex)
            {
                // Return a generic error response ...
                return StatusCode(500, $"An error occurred while logout. {ex.Message}");
            }
        }

        [HttpGet("GetUserById/{UserID}")]
        public IActionResult GetUserById(int UserID)
        {
            try
            {
                var user = _userService.GetUserById(UserID);
                return Ok(user);
            }
            catch (Exception ex)
            {
                // Return a generic error response ...
                return StatusCode(500, $"An error occurred while retrieving user. {ex.Message}");
            }
        }

        private void SetCookie(string name, string value, int? minutes = null, int? days = null)
        {
            var opts = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = minutes.HasValue
                    ? DateTimeOffset.UtcNow.AddMinutes(minutes.Value)
                    : days.HasValue
                        ? DateTimeOffset.UtcNow.AddDays(days.Value)
                        : DateTimeOffset.UtcNow.AddDays(7)
            };
            Response.Cookies.Append(name, value, opts);
        }

        private string GetIp() => HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    public sealed class LoginDto
    {
        [Required]
        public string UsernameOrEmail { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
