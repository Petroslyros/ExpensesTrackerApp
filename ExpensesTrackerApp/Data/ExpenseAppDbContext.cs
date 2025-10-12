using Microsoft.EntityFrameworkCore;

namespace ExpensesTrackerApp.Data
{
    public class ExpenseAppDbContext : DbContext
    {
        public ExpenseAppDbContext(DbContextOptions<ExpenseAppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Expense> Expenses { get; set; } = null!;
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; } = null!;

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
                      .OnDelete(DeleteBehavior.Cascade);

                // Expense ER
                entity.HasOne(d => d.ExpenseCategory)
                      .WithMany(p => p.Expenses)
                      .HasForeignKey(d => d.ExpenseCategoryId)
                      .OnDelete(DeleteBehavior.SetNull);
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
        }
    }
}
