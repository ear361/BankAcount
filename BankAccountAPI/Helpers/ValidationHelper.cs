using BankAccountAPI.Exceptions;

namespace BankAccountAPI.Helpers
{
    public static class ValidationHelper
    {
        public static void ValidateUsername(this string username)
        {
            if (string.IsNullOrWhiteSpace(username)) 
                throw new InvalidInputException($"Username cannot be empty. Username: {username}");
            if (username.Length > 10) 
                throw new InvalidInputException($"Username's maximum length is 10. Username: {username}");
        }
    }
}