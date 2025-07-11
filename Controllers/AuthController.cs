using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Supabase.Gotrue;
using UpShareBackend.supabaseAuth;

namespace UpShareBackend.Controllers
{
    [ApiController]
    [Route("login/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthModel _authModel;

        public AuthController()
        {
            _authModel = new AuthModel();
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] LoginRequest request)
        {
            try
            {
                var session = await _authModel.SignInAsync(request.Email, request.Password);

                // Transform Supabase session to match the frontend expected format
                var responseSession = new
                {
                    AccessToken = session.AccessToken,
                    RefreshToken = session.RefreshToken ?? string.Empty,
                    ExpiresAt = session.ExpiresAt(),
                    User = new
                    {
                        Id = session.User?.Id ?? string.Empty,
                        Email = session.User?.Email ?? string.Empty,
                        Username = session.User?.UserMetadata.TryGetValue(
                            "username",
                            out var username
                        ) == true
                            ? username.ToString()
                            : string.Empty,
                        EmailConfirmed = session.User?.EmailConfirmedAt.HasValue ?? false,
                    },
                };

                return Ok(responseSession);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignupRequest request)
        {
            try
            {
                var session = await _authModel.SignUpAsync(request.Email, request.Password);

                // Transform Supabase session to match the frontend expected format
                var responseSession = new
                {
                    AccessToken = session.AccessToken,
                    RefreshToken = session.RefreshToken ?? string.Empty,
                    ExpiresAt = session.ExpiresAt(),
                    User = new
                    {
                        Id = session.User?.Id ?? string.Empty,
                        Email = session.User?.Email ?? string.Empty,
                        Username = session.User?.UserMetadata.TryGetValue(
                            "username",
                            out var username
                        ) == true
                            ? username.ToString()
                            : string.Empty,
                        EmailConfirmed = session.User?.EmailConfirmedAt.HasValue ?? false,
                    },
                };

                return Ok(responseSession);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPost("signout")]
        public async Task<IActionResult> SignOut()
        {
            try
            {
                await _authModel.SignOutAsync();
                return Ok(new { Message = "Successfully signed out" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmation([FromBody] EmailRequest request)
        {
            try
            {
                await _authModel.ResendConfirmationEmail(request.Email);
                return Ok(new { Message = "Confirmation email sent" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] EmailRequest request)
        {
            try
            {
                await _authModel.SendForgetPassword(request.Email);
                return Ok(new { Message = "Password reset email sent" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] PasswordUpdateRequest request)
        {
            try
            {
                var model = new EnterNewPasswordViewModel
                {
                    Token = request.Token,
                    NewPassword = request.NewPassword,
                };

                var session = await _authModel.UpdateUserPassword(model);

                // Transform Supabase session to match the frontend expected format
                var responseSession = new
                {
                    AccessToken = session.AccessToken,
                    RefreshToken = session.RefreshToken ?? string.Empty,
                    ExpiresAt = session.ExpiresAt(),
                    User = new
                    {
                        Id = session.User?.Id ?? string.Empty,
                        Email = session.User?.Email ?? string.Empty,
                        Username = session.User?.UserMetadata.TryGetValue(
                            "username",
                            out var username
                        ) == true
                            ? username.ToString()
                            : string.Empty,
                        EmailConfirmed = session.User?.EmailConfirmedAt.HasValue ?? false,
                    },
                };

                return Ok(responseSession);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }

    // Request Models
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class SignupRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class EmailRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    public class PasswordUpdateRequest
    {
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
