using ExpensesTrackerApp.DTO;
using ExpensesTrackerApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpensesTrackerApp.Controllers
{
    public class ExpensesController : BaseController
    {
        private readonly IConfiguration configuration;
        // IConfiguration (used for reading settings if needed)

        public ExpensesController(IApplicationService applicationService, IConfiguration configuration) :
            base(applicationService)
        {
            this.configuration = configuration;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseReadOnlyDTO>> GetExpenseById(int id)
        {
            var expense = await applicationService.ExpenseService.GetExpenseByIdAsync(id);
            if (expense == null)
                return NotFound();

            return Ok(expense);
        }

        [HttpPost]
        [Authorize]// Requires a valid JWT token (user must be logged in)
        public async Task<ActionResult<ExpenseReadOnlyDTO>> CreateExpense([FromBody] ExpenseInsertDTO expenseDto)
        {
            // Checks if the request body (DTO) passed model validation rules
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verifies that the user is authenticated
            if (AppUser == null)
                return Unauthorized("User must be logged in to create an expense.");

            // Calls service to create the expense for the logged-in user
            var createdExpense = await applicationService.ExpenseService.CreateExpenseAsync(expenseDto, AppUser.Id);

            // Returns HTTP 201 (Created) with location header (GetExpenseById) and the new expense
            return CreatedAtAction(nameof(GetExpenseById), new { id = createdExpense.Id }, createdExpense);
        }

        [HttpDelete("{id}")]
        [Authorize] // Requires authentication
        public async Task<IActionResult> DeleteExpense(int id)
        {
            // Checks if user info exists in the controller (token decoded)
            if (AppUser == null)
                return Unauthorized("User must be logged in to delete an expense.");

            // Calls service to delete the expense (handles ownership & validation internally)
            await applicationService.ExpenseService.DeleteExpenseAsync(id, AppUser.Id);

            //200
            return Ok(new { message = "Expense deleted successfully." });
        }




    }
}
