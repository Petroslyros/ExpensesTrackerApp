using System.ComponentModel.DataAnnotations;

namespace ExpensesTrackerApp.DTO
{
    public record UserLoginDTO
    {
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Username must be between " +
            "2 and 50 characters.")]
        public string? Username { get; set; }

        [RegularExpression(@"(?=.*?[A-Z])(?=.*?[a-z])(?=.*?\d)(?=.*?\W)^.{8,}$",
            ErrorMessage = "Password must contain at least one uppercase, one lowercase, " +
            "one digit, and one special character")]
        public string? Password { get; set; }

        public bool KeepLoggedIn { get; set; }
    }
}
