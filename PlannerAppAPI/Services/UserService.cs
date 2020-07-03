using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PlannerAppAPI.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PlannerAppAPI.Services
{
    public class UserService : IUserService
    {
        private UserManager<ApplicationUser> _userManager;
        private IConfiguration _configuration;
        private IMailService _mailService;

        public UserService(UserManager<ApplicationUser> userManager, IConfiguration configuration, IMailService mailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mailService = mailService;
        }

        public async Task<UserManagerResponse> RegisterUserAsync(RegisterRequest registerRequest)
        {
            if (registerRequest == null)
            {
                throw new NullReferenceException("Register request is null");
            }

            if (registerRequest.Password != registerRequest.ConfirmPassword)
            {
                return new UserManagerResponse
                {
                    Message = "Confirm password does not match the password",
                    IsSuccess = false,
                };
            }

            var applicationUser = new ApplicationUser
            {
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                Email = registerRequest.Email,
                UserName = registerRequest.Email,
            };

            var result = await _userManager.CreateAsync(applicationUser, registerRequest.Password);

            if (result.Succeeded)
            {
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{_configuration["AppUrl"]}/api/auth/confirmemail?userid={applicationUser.Id}&token={validEmailToken}";

                await _mailService.SendEmailAsync(applicationUser.Email, "Email confirmation", $"<h1>Welcome to PlannerApp!</h1>" +
                    $"<p>To confirm your email, <a href='{url}'>Click here</a></p>");

                return new UserManagerResponse
                {
                    Message = "User creation successful",
                    IsSuccess = true,
                };
            }

            return new UserManagerResponse
            {
                Message = "User creation failed",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagerResponse> LoginUserAsync(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);

            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = "There is no account associated with that Email address",
                    IsSuccess = false,
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

            if (!result)
            {
                return new UserManagerResponse
                {
                    Message = "Incorrect password",
                    IsSuccess = false,
                };
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, loginRequest.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["AuthSettings:Issuer"],
                audience: _configuration["AuthSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return new UserManagerResponse
            {
                Message = tokenAsString,
                IsSuccess = true,
                ExpireDate = token.ValidTo
            };
        }

        public async Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = "User not found",
                    IsSuccess = false,
                };
            }

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
            {
                return new UserManagerResponse
                {
                    Message = "Email confirmation successful",
                    IsSuccess = true,
                };
            }

            return new UserManagerResponse
            {
                Message = "Email confirmation failed",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagerResponse> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = "There is no account associated with that Email address",
                    IsSuccess = false,
                };
            }

            var passwordResetTtoken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedPasswordResetToken = Encoding.UTF8.GetBytes(passwordResetTtoken);
            var validPasswordResetToken = WebEncoders.Base64UrlEncode(encodedPasswordResetToken);

            string url = $"{_configuration["AppUrl"]}/ResetPassword?email={email}&token={validPasswordResetToken}";

            await _mailService.SendEmailAsync(email, "Reset Password", "<h1>PlannerApp - Password Reset</h1>" +
                $"<p>To reset your password, <a href={url}>Click here</a></p>");

            return new UserManagerResponse
            {
                Message = "Reset Password email sent successfully",
                IsSuccess = true,
            };
        }

        public async Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordRequest.Email);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = "There is no account associated with that Email address",
                    IsSuccess = false,
                };
            }

            if (resetPasswordRequest.NewPassword != resetPasswordRequest.ConfirmPassword)
            {
                return new UserManagerResponse
                {
                    Message = "Confirm password does not match the password",
                    IsSuccess = false,
                };
            }

            var decodedToken = WebEncoders.Base64UrlDecode(resetPasswordRequest.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ResetPasswordAsync(user, normalToken, resetPasswordRequest.NewPassword);

            if (result.Succeeded)
            {
                return new UserManagerResponse
                {
                    Message = "Password reset successful",
                    IsSuccess = true,
                };
            }

            return new UserManagerResponse
            {
                Message = "Password reset failed",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description),
            };
        }
    }
}
