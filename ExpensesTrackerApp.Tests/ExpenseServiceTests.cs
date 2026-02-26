using AutoMapper;
using ExpensesTrackerApp.Data;
using ExpensesTrackerApp.DTO;
using ExpensesTrackerApp.Exceptions;
using ExpensesTrackerApp.Repositories.Interfaces;
using ExpensesTrackerApp.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ExpensesTrackerApp.Tests
{
    public class ExpenseServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ExpenseService>> _loggerMock;
        private readonly ExpenseService _service;

        public ExpenseServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ExpenseService>>();

            // Φτιάχνουμε το service δίνοντάς του τα Mock αντικείμενα (.Object)
            _service = new ExpenseService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }


        [Fact]
        public async Task CreateExpenseAsync_ShouldThrowException_WhenCategoryNameIsEmpty()
        {
            // 1. ARRANGE
            int userId = 1;
            // Δημιουργούμε ένα DTO με κενό CategoryName για να προκαλέσουμε το Exception
            var inputDto = new ExpenseInsertDTO("Lunch", 10.5m, DateTime.Now, "");

            // Φτιάχνουμε το Mock του Interface (πλέον δουλεύει τέλεια χωρίς constructors!)
            var userRepoMock = new Mock<IUserRepository>();

            // Λέμε στο Mock του UserRepo να επιστρέψει έναν χρήστη όταν κληθεί η GetAsync
            // Αυτό χρειάζεται για να "περάσει" ο κώδικας τον πρώτο έλεγχο στο Service
            userRepoMock.Setup(r => r.GetAsync(userId))
                        .ReturnsAsync(new User { Id = userId, Username = "Petros" });

            // Συνδέουμε το Mock Repo με το Mock UnitOfWork
            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(userRepoMock.Object);

            // 2. ACT & 3. ASSERT
            // Επιβεβαιώνουμε ότι το Service πετάει InvalidArgumentException λόγω κενού CategoryName
            await Assert.ThrowsAsync<InvalidArgumentException>(() =>
                _service.CreateExpenseAsync(inputDto, userId));
        }

        [Fact]
        public async Task CreateExpenseAsync_ShouldReturnReadOnlyDto_WhenInputIsValid()
        {
            // 1. ARRANGE
            int userId = 1;
            var inputDto = new ExpenseInsertDTO("Gym Subscription", 50m, DateTime.Now, "Health");
            var user = new User { Id = userId, Username = "Petros" };
            var category = new ExpenseCategory { Id = 10, Name = "Health" };
            var expenseEntity = new Expense { Id = 1, Title = "Gym Subscription", Amount = 50m };
            var expectedReturnDto = new ExpenseReadOnlyDTO { Id = 1, Title = "Gym Subscription", Amount = 50m };

            var userRepoMock = new Mock<IUserRepository>();
            var categoryRepoMock = new Mock<IExpenseCategoryRepository>();
            var expenseRepoMock = new Mock<IExpenseRepository>();

            // Setup Unit of Work
            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(userRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.ExpenseCategoryRepository).Returns(categoryRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.ExpenseRepository).Returns(expenseRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(true);

            // Setup Repositories logic
            userRepoMock.Setup(r => r.GetAsync(userId)).ReturnsAsync(user);
            categoryRepoMock.Setup(r => r.GetByNameAsync("Health")).ReturnsAsync(category);

            // Setup Mapper
            _mapperMock.Setup(m => m.Map<Expense>(inputDto)).Returns(expenseEntity);
            _mapperMock.Setup(m => m.Map<ExpenseReadOnlyDTO>(expenseEntity)).Returns(expectedReturnDto);

            // 2. ACT
            var result = await _service.CreateExpenseAsync(inputDto, userId);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.Equal("Gym Subscription", result.Title);
            // Επιβεβαιώνουμε ότι καλέστηκε η SaveAsync στο Unit of Work
            _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
        }

    }
}