namespace ExpensesTrackerApp.Exceptions
{
    public class EntityForbiddenException : AppException
    {
        private static readonly string DEFAULT_CODE = "Forbidden";


        public EntityForbiddenException(string code, string message) : base(code + DEFAULT_CODE, message) { }

    }
}
