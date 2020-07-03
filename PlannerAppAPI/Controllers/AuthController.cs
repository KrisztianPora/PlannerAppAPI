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
        private IMailService _mailService;

        public AuthController(IUserService userService, IMailService mailService)
        {
            _userService = userService;
            _mailService = mailService;
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

        // /api/auth/confirmemail?userid&token
        [HttpGet("ConfirmEmail")]
        [ProducesResponseType(200, Type = typeof(UserManagerResponse))]
        [ProducesResponseType(400, Type = typeof(UserManagerResponse))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return NotFound(); // Status code: 404
            }

            var result = await _userService.ConfirmEmailAsync(userId, token);

            if (result.IsSuccess)
            {
                return Ok(result); // Status code: 200
            }

            return BadRequest(result); // Status code: 400
        }

        // /api/auth/forgetpassword
        [HttpPost("ForgetPassword")]
        [ProducesResponseType(200, Type = typeof(UserManagerResponse))]
        [ProducesResponseType(400, Type = typeof(UserManagerResponse))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return NotFound(); // Status code: 404
            }

            var result = await _userService.ForgetPasswordAsync(email);

            if (result.IsSuccess)
            {
                return Ok(result); // Status code: 200
            }

            return BadRequest(result); // Status code: 400
        }

        // /api/auth/resetpassword
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm]ResetPasswordRequest resetPasswordRequest)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.ResetPasswordAsync(resetPasswordRequest);

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
