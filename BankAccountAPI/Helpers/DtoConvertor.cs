using BankAccountAPI.DTO;
using BankAccountAPI.Models;

namespace BankAccountAPI.Helpers
{
    public static class DtoConvertor
    {
        public static AccountDTO ToDto(this Account account)
        {
            return new AccountDTO
            {
                AccountNumber = account.AccountNumber,
                Balance = account.Balance,
                Name = account.Name
            };
        }
    }
}