using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PlannerAppAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlannerAppAPI.Services
{
    public class UserService : IUserService
    {
        private UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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
    }
}
