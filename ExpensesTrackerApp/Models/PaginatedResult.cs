namespace ExpensesTrackerApp.Models
{
    public class PaginatedResult<T>
    {
        public List<T> Data { get; set; } = [];
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);

        public PaginatedResult() { }

        public PaginatedResult(List<T> data, int totalRecrods, int pageNumber, int pageSize)
        {
            Data = data;
            TotalRecords = totalRecrods;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
