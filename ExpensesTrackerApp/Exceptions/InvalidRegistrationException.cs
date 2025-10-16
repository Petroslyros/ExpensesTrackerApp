namespace SchoolApp.Exceptions
{
    public class InvalidRegistrationException : Exception
    {
        public InvalidRegistrationException() { }
        public InvalidRegistrationException(string message) : base(message) { }
        public InvalidRegistrationException(string message, Exception inner) : base(message, inner) { }
    }
}
