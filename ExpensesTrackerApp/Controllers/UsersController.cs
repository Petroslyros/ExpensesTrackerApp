using ExpensesTrackerApp.Core.Filters;
using ExpensesTrackerApp.DTO;
using ExpensesTrackerApp.Exceptions;
using ExpensesTrackerApp.Models;
using ExpensesTrackerApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpensesTrackerApp.Controllers
{


    public class UsersController : BaseController
    {
        private readonly IConfiguration configuration;

        public UsersController(IApplicationService applicationService, IConfiguration configuration) :
            base(applicationService)
        {
            this.configuration = configuration;
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserReadOnlyDTO>> GetUserById(int id)
        {
            // If the currently logged-in user is not the requested user and not Admin, forbid access
            if (AppUser?.Id != id && !User.IsInRole("Admin"))
                return Forbid();

            UserReadOnlyDTO userReadOnlyDTO = await applicationService.UserService.GetUserByIdAsync(id)
                ?? throw new EntityNotFoundException("User", "User with id " + id + " not found");

            return Ok(userReadOnlyDTO); // Return 200 OK with user data
        }

        [HttpPost]
        //[AllowAnonymous]
        public async Task<ActionResult<UserReadOnlyDTO>> RegisterUser([FromBody] UserRegisterDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Validation failed, return 400
            }

            var registeredUser = await applicationService.UserService.RegisterUserAsync(registerDto);

            // Return 201 Created with location header pointing to the new user
            return CreatedAtAction(
                nameof(GetUserById),
                new { id = registeredUser.Id },
                registeredUser
            );
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await applicationService.UserService.DeleteUserAsync(id);
            if (!result) return NotFound(); // User not found, return 404

            // Return 200 OK with confirmation message
            return Ok(new { message = $"User with ID {id} was successfully deleted." });

        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserReadOnlyDTO>> UpdateUser(int id, [FromBody] UpdateUserDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Validation failed, return 400

            var updatedUser = await applicationService.UserService.UpdateUserAsync(id, dto);

            return Ok(updatedUser);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserReadOnlyDTO>> GetByUsername([FromQuery] string username)
        {
            if (AppUser?.Username != username && !User.IsInRole("Admin"))
                throw new EntityForbiddenException("User", "You can only access your own profile.");

            // Service already returns UserReadOnlyDTO
            var userDto = await applicationService.UserService.GetUserByUsernameAsync(username)
                          ?? throw new EntityNotFoundException("User", $"User: {username} not found");

            return Ok(userDto);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaginatedResult<UserReadOnlyDTO>>> GetPaginatedUsersFiltered(
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 10,
                [FromQuery] string? username = null,
                [FromQuery] string? email = null,
                [FromQuery] string? userRole = null)
        {
            // Create filter DTO from query parameters
            var filters = new UserFiltersDTO
            {
                UserName = username,
                Email = email,
                UserRole = userRole
            };
            // Fetch filtered, paginated result
            var result = await applicationService.UserService.GetPaginatedUsersFilteredAsync(pageNumber, pageSize, filters);

            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaginatedResult<UserReadOnlyDTO>>> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await applicationService.UserService.GetAllUsersAsync(pageNumber, pageSize);

            if (result == null || !result.Data.Any())
                return NotFound("No users found.");

            return Ok(result);
        }
    }
}