using ExpensesTrackerApp.Core.Enums;

namespace ExpensesTrackerApp.Data
{
    public class User : BaseEntity
    {
        public int Id { get; set; }
        public string? Username { get; set; } = null;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public UserRole UserRole { get; set; }

        //navigation key
        public virtual ICollection<Expense> Expenses { get; set; } = new HashSet<Expense>();


    }
}
