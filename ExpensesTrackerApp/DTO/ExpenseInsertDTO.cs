using System.ComponentModel.DataAnnotations;

namespace ExpensesTrackerApp.DTO
{
    public record ExpenseInsertDTO
    (
          [Required(ErrorMessage ="Title is required")]
          string Title,
          [Range(1, double.MaxValue, ErrorMessage ="Amount must be greater than 0")]
          decimal Amount,

          DateTime Date,

          [Required(ErrorMessage = "Category name is required")]
          string CategoryName
    );

}
