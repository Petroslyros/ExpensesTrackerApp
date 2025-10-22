namespace ExpensesTrackerApp.DTO
{
    public class BudgetReadOnlyDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public decimal LimitAmount { get; set; }
        public decimal? SpentAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
