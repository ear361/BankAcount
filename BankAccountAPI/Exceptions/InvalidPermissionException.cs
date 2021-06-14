using System.Net;

namespace BankAccountAPI.Exceptions
{
    public class InvalidPermissionException : BaseCustomException
    {
        public InvalidPermissionException(string message) : base(message, (int) HttpStatusCode.Forbidden)
        {
        }
    }
}