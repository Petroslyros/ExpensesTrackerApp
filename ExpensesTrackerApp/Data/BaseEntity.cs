namespace ExpensesTrackerApp.Data
{
    public abstract class BaseEntity
    {
        public DateTime InsertedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
        public bool isDeleted { get; set; } = false; // soft delete
        public DateTime? DeletedAt { get; set; }
    }
}
