using System;
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
    public class PostTests
    {
        private readonly Mock<ILogger<AccountController>> _loggerMock;
        private const string username = "User1";
        private const int accountNumber = 11111111;

        public PostTests()
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

            var exception = await Record.ExceptionAsync(async () => await sut.Post(username, new AccountDTO(){Name = "name"}));
            Assert.IsType<ItemNotFoundException>(exception);
            Assert.Contains("User not found.", exception.Message);
        }
                
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task WhenAccountNameEmpty_ShouldThrowException(string accountName)
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new BankContext(options);
            context.Users.Add(new User() {Username = username});
            await context.SaveChangesAsync();
            var sut = new AccountController(context, _loggerMock.Object);

            var exception = await Record.ExceptionAsync(async () => await sut.Post(username, new AccountDTO(){Name = accountName}));
            Assert.IsType<InvalidInputException>(exception);
            Assert.Contains("New account's name cannot be empty.", exception.Message);
        }
        
        [Fact]
        public async Task WhenInputValid_ShouldCreateAccount()
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new BankContext(options);
            context.Users.Add(new User() {Username = username});
            await context.SaveChangesAsync();
            var sut = new AccountController(context, _loggerMock.Object);

            ActionResult<AccountDTO> result = null;
            var exception = await Record.ExceptionAsync(async () => result= await sut.Post(username, new AccountDTO(){Name = "New Account"}));
            Assert.Null(exception);
            var addResult = result.Result as CreatedResult;
            var dto = addResult.Value as AccountDTO;
            Assert.True(context.Accounts.Any(a => a.AccountNumber == dto.AccountNumber));
        }
    }
}