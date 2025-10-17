using AutoMapper;
using ExpensesTrackerApp.DTO;
using ExpensesTrackerApp.Exceptions;
using ExpensesTrackerApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpensesTrackerApp.Controllers
{
    #region documentation
    /// <summary>
    /// Handles login requests and issues JWT tokens for authenticated users.
    /// </summary>
    #endregion
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IApplicationService applicationService, IConfiguration configuration, IMapper mapper)
        : BaseController(applicationService)
    {
        #region documentation
        /// <summary>
        /// Authenticates user credentials and returns a JWT token for authorized access.
        /// </summary>
        /// <param name="credentials">User credentials including username and password.</param>
        /// <returns>A JWT token if authentication is successful.</returns>
        /// <response code="200">Returns the JWT token.</response>
        /// <response code="401">If the credentials are invalid.</response>
        /// <exception cref="EntityNotAuthorizedException">
        /// Thrown when username or password is invalid.
        /// </exception>
        #endregion
        [HttpPost("login/access-token")]
        public async Task<ActionResult<JwtTokenDTO>> Login([FromBody] UserLoginDTO credentials)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await applicationService.UserService.VerifyAndGetUserAsync(credentials)
                ?? throw new EntityNotAuthorizedException("User", "Invalid username or password");

            var token = applicationService.UserService.CreateUserToken(
                user.Id,
                user.Username!,
                user.Email!,
                user.UserRole,
                configuration["Authentication:SecretKey"]!
            );

            var jwtToken = new JwtTokenDTO
            {
                Token = token,
                Username = user.Username!,
                Role = user.UserRole.ToString(),
                ExpiresAt = DateTime.UtcNow.AddHours(3)
            };

            return Ok(jwtToken);
        }


    }
}
