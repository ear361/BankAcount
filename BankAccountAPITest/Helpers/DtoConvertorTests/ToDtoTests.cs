using BankAccountAPI.Helpers;
using BankAccountAPI.Models;
using Xunit;

namespace BankAccountAPITest.Helpers.DtoConvertorTests
{
    public class ToDtoTests
    {
        [Fact]
        public void WhenAccountIsNull_ShouldReturnNull()
        {
            Account account = null;
            Assert.Null(account.ToDto());
        }
        
        [Fact]
        public void WhenAccountIsNotNull_ShouldReturnDto()
        {
            Account account = new Account(){AccountNumber = 11111111, Balance = 100, Name = "Account Name"};
            var result = account.ToDto();
            Assert.Equal(11111111, result.AccountNumber);
            Assert.Equal(100, result.Balance);
            Assert.Equal("Account Name", result.Name);
        }
    }
}