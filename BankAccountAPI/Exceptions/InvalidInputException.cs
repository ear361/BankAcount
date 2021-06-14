using System.Net;

namespace BankAccountAPI.Exceptions
{
    public class InvalidInputException : BaseCustomException
    {
        public InvalidInputException(string message) : base(message, (int) HttpStatusCode.BadRequest)
        {
        }
    }
}