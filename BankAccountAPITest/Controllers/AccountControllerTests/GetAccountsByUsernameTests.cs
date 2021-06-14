using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankAccountAPI.Controllers;
using BankAccountAPI.DTO;
using BankAccountAPI.Exceptions;
using BankAccountAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BankAccountAPITest.Controllers.AccountControllerTests
{
    public class GetAccountsByUsernameTests
    {
        private readonly Mock<ILogger<AccountController>> _loggerMock;
        private const string username = "User1";
        private const int accountNumber = 11111111;

        public GetAccountsByUsernameTests()
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

            var exception = await Record.ExceptionAsync(async () => await sut.GetAccountsByUsername("name"));
            Assert.IsType<ItemNotFoundException>(exception);
            Assert.Contains("User not found.", exception.Message);
        }
        
        [Fact]
        public async Task WhenInputValid_ShouldReturnResult()
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new BankContext(options);
            context.Users.Add(new User() {Username = "Admin"});
            context.Users.Add(new User() {Username = username});
            context.Accounts.Add(new Account() {Username = "Admin", AccountNumber = accountNumber});
            context.Accounts.Add(new Account() {Username = username, AccountNumber = 22222222});
            context.Accounts.Add(new Account() {Username = username, AccountNumber = 33333333});
            await context.SaveChangesAsync();
            var sut = new AccountController(context, _loggerMock.Object);

            ActionResult<IEnumerable<AccountDTO>> result = null;
            var exception = await Record.ExceptionAsync(async () => result = await sut.GetAccountsByUsername(username));
            Assert.Null(exception);
            Assert.Equal(2, result.Value.Count());
            Assert.Contains(result.Value, a => a.AccountNumber == 22222222);
            Assert.Contains(result.Value, a => a.AccountNumber == 33333333);
        }
    }
}