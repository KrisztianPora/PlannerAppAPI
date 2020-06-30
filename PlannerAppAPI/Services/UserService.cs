using Microsoft.AspNetCore.Identity;
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

        public UserService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
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
    }
}
