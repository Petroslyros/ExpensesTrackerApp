The Expenses Tracker API is a clean and modular ASP.NET Core 8 Web API designed to help users manage their personal expenses and categories efficiently. It supports user authentication with JWT tokens, allowing each user to securely register, log in, and track their own expenses.

The project follows a layered architecture using the Repository and Service patterns for maintainability and scalability. Data transfer between layers is handled through DTOs and AutoMapper, while a custom middleware ensures consistent global error handling and structured logging with Serilog.

Built with Entity Framework Core and SQL Server, this API demonstrates best practices in backend development — including dependency injection, clean separation of concerns, and secure authentication — making it both practical and educational for real-world projects.
