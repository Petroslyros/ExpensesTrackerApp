namespace ExpensesTrackerApp.Exceptions
{
    public class EntityAlreadyExistsException : AppException
    {
        private static readonly string DEFAULT_CODE = "AlreadyExists";

        public EntityAlreadyExistsException(string code, string message)
            : base(code + DEFAULT_CODE, message)
        {
        }




    }
}
