namespace ExpensesTrackerApp.DTO
{
    public class BudgetInsertDTO
    {
        public int CategoryId { get; set; }
        public decimal LimitAmount { get; set; }  // Max allowed spending
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
