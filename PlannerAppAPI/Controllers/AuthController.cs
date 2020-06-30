using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlannerAppAPI.Models;
using PlannerAppAPI.Services;

namespace PlannerAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        // /api/auth/register
        [HttpPost("Register")]
        [ProducesResponseType(200, Type = typeof(UserManagerResponse))]
        [ProducesResponseType(400, Type = typeof(UserManagerResponse))]
        public async Task<IActionResult> RegisterAsync([FromBody]RegisterRequest registerRequest)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterUserAsync(registerRequest);

                if (result.IsSuccess)
                {
                    return Ok(result); // Status code: 200
                }

                return BadRequest(result);
            }

            return BadRequest(new UserManagerResponse
            {
                Message = "There are invalid values",
                IsSuccess = false,
            }); // Status code: 400
        }

        // /api/auth/login
        [HttpPost("Login")]
        [ProducesResponseType(200, Type = typeof(UserManagerResponse))]
        [ProducesResponseType(400, Type = typeof(UserManagerResponse))]
        public async Task<IActionResult> LoginAsync([FromBody]LoginRequest loginRequest)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginUserAsync(loginRequest);

                if (result.IsSuccess)
                {
                    return Ok(result); // Status code: 200
                }

                return BadRequest(result);
            }

            return BadRequest(new UserManagerResponse
            {
                Message = "There are invalid values",
                IsSuccess = false,
            }); // Status code: 400
        }
    }
}
