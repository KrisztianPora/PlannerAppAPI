using PlannerAppAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlannerAppAPI.Services
{
    public interface IUserService
    {
        Task<UserManagerResponse> RegisterUserAsync(RegisterRequest registerRequest);

        Task<UserManagerResponse> LoginUserAsync(LoginRequest loginRequest);
    }
}
