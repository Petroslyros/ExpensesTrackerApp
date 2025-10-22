namespace ExpensesTrackerApp.DTO
{
    public class ExpenseReadOnlyDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public ExpenseCategoryReadOnlyDTO? Category { get; set; }
    }
}