namespace ExpensesTrackerApp.Data
{
    public class Expense : BaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }


        // we set these to null so we dont delete we set the expense to null when the category is deleted
        public int? ExpenseCategoryId { get; set; } //fk
        public virtual ExpenseCategory? ExpenseCategory { get; set; } = null!; //navigation property

        //user 
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
