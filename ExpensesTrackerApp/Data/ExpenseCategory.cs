namespace ExpensesTrackerApp.Data
{
    public class ExpenseCategory : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        //navigation 
        public virtual ICollection<Expense> Expenses { get; set; } = new HashSet<Expense>();
    }
}
