using System;
using System.Net;

namespace BankAccountAPI.Exceptions
{
    public abstract class BaseCustomException : Exception
    {
        public int StatusCode { get;}

        protected BaseCustomException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}