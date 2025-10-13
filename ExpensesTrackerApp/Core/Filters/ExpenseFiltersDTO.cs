namespace ExpensesTrackerApp.Core.Filters
{
    public class ExpenseFiltersDTO
    {
        public int? CategoryId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string? Title { get; set; }
    }
}
