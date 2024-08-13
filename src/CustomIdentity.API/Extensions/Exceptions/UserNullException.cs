namespace CustomIdentity.API.Extensions.Exceptions
{
    [Serializable]
    internal class UserNullException : Exception
    {
        public UserNullException()
        {
        }

        public UserNullException(string? message) : base(message)
        {
        }

        public UserNullException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}