namespace ExpensesTrackerApp.DTO
{
    public record ExpenseReadOnlyDTO(

    int Id,
    string Title,
    decimal Amount,
    DateTime Date,
    ExpenseCategoryReadOnlyDTO? Category
);

}
