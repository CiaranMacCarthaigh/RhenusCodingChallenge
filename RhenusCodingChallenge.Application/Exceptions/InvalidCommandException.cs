namespace RhenusCodingChallenge.Application.Exceptions
{
    public class InvalidCommandException : Exception
    {
        public InvalidCommandException(string propertyName, string reason)
            : base("Command validation failed")
        {
            PropertyName = propertyName;
            Reason = reason;
        }

        public string PropertyName { get; }
        public string Reason { get; }
    }
}
