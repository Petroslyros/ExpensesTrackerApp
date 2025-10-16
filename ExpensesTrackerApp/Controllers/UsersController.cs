using ExpensesTrackerApp.DTO;
using ExpensesTrackerApp.Exceptions;
using ExpensesTrackerApp.Services.Interfaces;
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
        public async Task<ActionResult<UserReadOnlyDTO>> GetUserById(int id)
        {
            UserReadOnlyDTO userReadOnlyDTO = await applicationService.UserService.GetUserByIdAsync(id)
                ?? throw new EntityNotFoundException("User", "User with id " + id + " not found");

            return Ok(userReadOnlyDTO);
        }



    }
}