namespace ExpensesTrackerApp.DTO
{
    public record ExpenseInsertDTO(
    string Title,
    decimal Amount,
    DateTime Date,
    string CategoryName     // typed by the user
);


}
