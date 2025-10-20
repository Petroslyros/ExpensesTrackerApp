using ExpensesTrackerApp.DTO;
using ExpensesTrackerApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpensesTrackerApp.Controllers
{
    public class ExpensesController : BaseController
    {
        private readonly IConfiguration configuration;

        public ExpensesController(IApplicationService applicationService, IConfiguration configuration) :
            base(applicationService)
        {
            this.configuration = configuration;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ExpenseReadOnlyDTO>> CreateExpense([FromBody] ExpenseInsertDTO expenseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (AppUser == null)
                return Unauthorized("User must be logged in to create an expense.");

            var createdExpense = await applicationService.ExpenseService.CreateExpenseAsync(expenseDto, AppUser.Id);

            return CreatedAtAction(nameof(GetExpenseById), new { id = createdExpense.Id }, createdExpense);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseReadOnlyDTO>> GetExpenseById(int id)
        {
            var expense = await applicationService.ExpenseService.GetExpenseByIdAsync(id);
            if (expense == null)
                return NotFound();

            return Ok(expense);
        }


    }
}
