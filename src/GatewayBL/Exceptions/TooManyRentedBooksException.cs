namespace GatewayBL.Exceptions;

public class TooManyRentedBooksException : Exception
{
    public TooManyRentedBooksException() { }

    public TooManyRentedBooksException(string message) : base(message) { }

    public TooManyRentedBooksException(string message, Exception innerException)
        : base(message, innerException) { }
}