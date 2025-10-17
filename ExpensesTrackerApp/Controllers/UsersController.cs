using ExpensesTrackerApp.DTO;
using ExpensesTrackerApp.Exceptions;
using ExpensesTrackerApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpensesTrackerApp.Controllers
{

    /// <summary>
    /// Controller for managing users 
    /// </summary>
    [Route("api/users/")]
    public class UsersController : BaseController
    {
        private readonly IConfiguration configuration;

        public UsersController(IApplicationService applicationService, IConfiguration configuration) :
            base(applicationService)
        {
            this.configuration = configuration;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<UserReadOnlyDTO>> GetUserById(int id)
        {
            if (AppUser?.Id != id && !User.IsInRole("Admin"))
                return Forbid("You can only access your own profile.");


            UserReadOnlyDTO userReadOnlyDTO = await applicationService.UserService.GetUserByIdAsync(id)
                ?? throw new EntityNotFoundException("User", "User with id " + id + " not found");

            return Ok(userReadOnlyDTO);
        }

        [HttpPost("register")]
        //[AllowAnonymous]
        public async Task<ActionResult<UserReadOnlyDTO>> RegisterUser([FromBody] UserRegisterDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var registeredUser = await applicationService.UserService.RegisterUserAsync(registerDto);

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = registeredUser.Id },
                registeredUser
            );
        }



    }
}