namespace ExpensesTrackerApp.Data
{
    public class Budget : BaseEntity
    {
        public int Id { get; set; }
        public decimal LimitAmount { get; set; }
        public decimal? SpentAmount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        //fk
        public int UserId { get; set; }
        public int CategoryId { get; set; }

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual ExpenseCategory Category { get; set; } = null!;
    }
}
