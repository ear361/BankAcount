using System;
using System.Linq;
using System.Threading.Tasks;
using BankAccountAPI.Contracts;
using BankAccountAPI.Controllers;
using BankAccountAPI.Exceptions;
using BankAccountAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BankAccountAPITest.Controllers.AccountControllerTests
{
    public class DepositTests
    {
        private readonly Mock<ILogger<AccountController>> _loggerMock;
        private const string username = "User1";
        private const int accountNumber = 11111111;

        public DepositTests()
        {
            _loggerMock = new Mock<ILogger<AccountController>>();
        }
        
        [Fact]
        public async Task WhenUserNotfound_ShouldThrowException()
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new BankContext(options);
            var sut = new AccountController(context, _loggerMock.Object);

            var exception = await Record.ExceptionAsync(async () => await sut.Deposit("name", new TransactionContract(){AccountNumber = accountNumber, Amount = 10}));
            Assert.IsType<ItemNotFoundException>(exception);
            Assert.Contains("User not found.", exception.Message);
        }
                
        [Fact]
        public async Task WhenTransactionAmountLessThan0_ShouldThrowException()
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new BankContext(options);
            context.Users.Add(new User() {Username = username});
            await context.SaveChangesAsync();
            var sut = new AccountController(context, _loggerMock.Object);

            var exception = await Record.ExceptionAsync(async () => await sut.Deposit(username, new TransactionContract(){AccountNumber = accountNumber, Amount = 0}));
            Assert.IsType<InvalidInputException>(exception);
            Assert.Contains("Transaction amount should be greater than 0", exception.Message);
        }
                
        [Fact]
        public async Task WhenAccountNotFound_ShouldThrowException()
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new BankContext(options);
            context.Users.Add(new User() {Username = username});
            await context.SaveChangesAsync();
            var sut = new AccountController(context, _loggerMock.Object);

            var exception = await Record.ExceptionAsync(async () => await sut.Deposit(username, new TransactionContract(){AccountNumber = accountNumber, Amount = 10}));
            Assert.IsType<ItemNotFoundException>(exception);
            Assert.Contains("Account not found.", exception.Message);
        } 
        
        [Fact]
        public async Task WhenInputValid_ShouldUpdateBalance()
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new BankContext(options);
            context.Users.Add(new User() {Username = username});
            context.Accounts.Add(new Account() {Username = username, AccountNumber = accountNumber, Balance = 100});
            await context.SaveChangesAsync();
            var sut = new AccountController(context, _loggerMock.Object);
            
            var exception = await Record.ExceptionAsync(async () => await sut.Deposit(username, new TransactionContract(){AccountNumber = accountNumber, Amount = 10}));
            Assert.Null(exception);
            var updatedBalance = context.Accounts.FirstOrDefault(a => a.AccountNumber == 11111111).Balance;
            Assert.Equal(100 + 10, updatedBalance);
        }
    }
}