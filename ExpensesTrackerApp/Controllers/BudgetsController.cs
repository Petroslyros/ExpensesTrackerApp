using ExpensesTrackerApp.DTO;
using ExpensesTrackerApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpensesTrackerApp.Controllers
{

    public class BudgetsController : BaseController
    {
        private readonly IConfiguration configuration;
        public BudgetsController(IApplicationService applicationService, IConfiguration configuration) : base(applicationService)
        {
            this.configuration = configuration;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BudgetReadOnlyDTO>> GetBudgetById(int id)
        {
            var budget = await applicationService.BudgetService.GetBudgetByIdAsync(id);
            if (budget == null) return NotFound();

            return Ok(budget);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BudgetReadOnlyDTO>> CreateBudget([FromBody] BudgetInsertDTO budgetDto)
        {
            // Checks if the request body (DTO) passed model validation rules
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verifies that the user is authenticated
            if (AppUser == null)
                return Unauthorized("User must be logged in to create an expense.");

            var createdBudget = await applicationService.BudgetService.CreateBudgetAsync(AppUser.Id, budgetDto);

            return CreatedAtAction(nameof(GetBudgetById), new { id = createdBudget.Id }, createdBudget);

        }

    }
}
