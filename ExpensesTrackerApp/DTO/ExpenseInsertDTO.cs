namespace ExpensesTrackerApp.DTO
{
    public record ExpenseInsertDTO(

        string Title,
        decimal Amount,
        DateTime Date,
        int UserId,
        int ExpenseCategoryId

    );


}
