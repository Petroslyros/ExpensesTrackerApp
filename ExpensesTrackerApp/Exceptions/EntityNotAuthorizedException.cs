namespace ExpensesTrackerApp.Exceptions
{
    public class EntityNotAuthorizedException : AppException
    {
        private static readonly string DEFAULT_CODE = "NotAuthorized";

        public EntityNotAuthorizedException(string code, string message)
            : base(code + DEFAULT_CODE, message)
        {
        }
    }
}