using System;

namespace BankAccountAPI.Helpers
{
    public static class ValidationHelper
    {
        public static void ValidateUsername(this string username)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new Exception("Username cannot be empty");
            if (username.Length > 10) throw new Exception("Invalid username");
        }
    }
}