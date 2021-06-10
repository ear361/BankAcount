using System.Threading.Tasks;
using BankAccountAPI.Controllers;
using BankAccountAPI.DTO;
using BankAccountAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BankAccountAPITest.Controllers
{
    public class UserController_PostTest
    {
        [Fact]
        public async Task WhenUserNotFound_ShouldThrowException()
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(databaseName: "WhenUserNotFound_ShouldThrowException")
                .Options;

            var context = new BankContext(options);

            var sut = new UserController(context);

            var exception = await Record.ExceptionAsync(async () => await sut.Post("username", new UserDTO()));
            Assert.NotNull(exception);
            Assert.Equal("User not found.", exception.Message);
        } 
        
        [Fact]
        public async Task WhenUserIsNotAdmin_ShouldThrowException()
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(databaseName: "WhenUserIsNotAdmin_ShouldThrowException")
                .Options;

            var context = new BankContext(options);
            context.Users.Add(new User() {Username = "username"});
            context.SaveChangesAsync();

            var sut = new UserController(context);

            var exception = await Record.ExceptionAsync(async () => await sut.Post("username", new UserDTO()));
            Assert.NotNull(exception);
            Assert.Equal("Only admin users can create new users.", exception.Message);
        }
        
        [Fact]
        public async Task WhenUsernameTooLong_ShouldThrowException()
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(databaseName: "WhenUsernameTooLong_ShouldThrowException")
                .Options;

            var context = new BankContext(options);
            context.Users.Add(new User() {Username = "Admin"});
            context.SaveChangesAsync();

            var sut = new UserController(context);

            var exception = await Record.ExceptionAsync(async () => await sut.Post("Admin", new UserDTO(){Username = "verylongname"}));
            Assert.NotNull(exception);
            Assert.Equal("Username's maximum length is 10.", exception.Message);
        } 
        
        [Fact]
        public async Task WhenUsernameAlreadyExist_ShouldThrowException()
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(databaseName: "WhenUsernameAlreadyExist_ShouldThrowException")
                .Options;

            var context = new BankContext(options);
            context.Users.Add(new User() {Username = "Admin"});
            context.Users.Add(new User() {Username = "username"});
            context.SaveChangesAsync();

            var sut = new UserController(context);

            var exception = await Record.ExceptionAsync(async () => await sut.Post("Admin", new UserDTO(){Username = "username"}));
            Assert.NotNull(exception);
            Assert.Equal("Username already exists.", exception.Message);
        } 
        
        [Fact]
        public async Task WhenAllValid_ShouldReturnCreated()
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(databaseName: "WhenUsernameAlreadyExist_ShouldThrowException")
                .Options;

            var context = new BankContext(options);
            context.Users.Add(new User() {Username = "Admin"});
            context.SaveChangesAsync();

            var sut = new UserController(context);

            ActionResult<UserDTO> result = null;
            var exception = await Record.ExceptionAsync(async () => result = await sut.Post("Admin", new UserDTO(){Username = "username"}));
            Assert.Null(exception);
            Assert.NotNull(result);
            var createdResult = result.Result as CreatedResult;
            Assert.IsType<UserDTO>(createdResult.Value);
        } 
    }
}