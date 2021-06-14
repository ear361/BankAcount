using System.Net;

namespace BankAccountAPI.Exceptions
{
    public class ItemNotFoundException : BaseCustomException
    {
        public ItemNotFoundException(string message) : base(message, (int) HttpStatusCode.NotFound)
        {
        }
    }
}