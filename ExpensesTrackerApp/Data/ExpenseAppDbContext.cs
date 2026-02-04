using Microsoft.EntityFrameworkCore;

namespace ExpensesTrackerApp.Data
{
    public class ExpenseAppDbContext : DbContext
    {
        public ExpenseAppDbContext(DbContextOptions<ExpenseAppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Expense> Expenses { get; set; } = null!;
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; } = null!;
        public DbSet<Budget> Budgets { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Username).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.Password).HasMaxLength(255);
                entity.Property(e => e.Firstname).HasMaxLength(255);
                entity.Property(e => e.Lastname).HasMaxLength(255);

                entity.Property(e => e.UserRole)
                .HasConversion<string>() // store enum as a string
                .HasMaxLength(20);

                entity.Property(e => e.InsertedAt)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(e => e.Email, "IX_Users_Email").IsUnique();
                entity.HasIndex(e => e.Username, "IX_Users_Username").IsUnique();
            });

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.ToTable("Expenses");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(e => e.Amount)
                      .HasColumnType("decimal(10,2)");

                entity.Property(e => e.Date)
                      .IsRequired();

                entity.Property(e => e.InsertedAt)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.ModifiedAt)
                      .ValueGeneratedOnAddOrUpdate()
                      .HasDefaultValueSql("GETUTCDATE()");

                // User ER
                entity.HasOne(d => d.User)
                      .WithMany(p => p.Expenses)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade); // Delete Expense with User

                // Expense ER
                entity.HasOne(d => d.ExpenseCategory)
                      .WithMany(p => p.Expenses)
                      .HasForeignKey(d => d.ExpenseCategoryId)
                      .OnDelete(DeleteBehavior.SetNull); // If expensecategory is deleted do not delete the expense
            });

            modelBuilder.Entity<ExpenseCategory>(entity =>
            {
                entity.ToTable("ExpenseCategories");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.InsertedAt)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.ModifiedAt)
                      .ValueGeneratedOnAddOrUpdate()
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(e => e.Name, "IX_ExpenseCategories_Name").IsUnique();
            });

            modelBuilder.Entity<Budget>(entity =>
            {
                entity.ToTable("Budgets");
                entity.HasKey(b => b.Id);

                entity.Property(b => b.LimitAmount)
                      .HasColumnType("decimal(10,2)")
                      .IsRequired();

                entity.Property(b => b.SpentAmount)
                      .HasColumnType("decimal(10,2)");

                entity.Property(b => b.StartDate)
                        .IsRequired();

                entity.Property(b => b.EndDate)
                      .IsRequired();

                // User relationship
                entity.HasOne(b => b.User)
                      .WithMany(u => u.Budgets)
                      .HasForeignKey(b => b.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Category relationship
                entity.HasOne(b => b.Category)
                      .WithMany()
                      .HasForeignKey(b => b.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict); // Do not Cascade delete

                //Composite Unique index to Ensure one budget per user per category
                entity.HasIndex(b => new { b.UserId, b.CategoryId }).IsUnique();

                entity.Property(b => b.InsertedAt)
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(b => b.ModifiedAt)
                      .ValueGeneratedOnAddOrUpdate()
                      .HasDefaultValueSql("GETUTCDATE()");
            });


        }
    }
}
