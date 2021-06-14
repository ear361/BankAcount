using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BankAccountAPI.Controllers;
using BankAccountAPI.DTO;
using BankAccountAPI.Exceptions;
using BankAccountAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace BankAccountAPITest.Controllers.UserControllerTests
{
    public class PostTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private Mock<ILogger<UserController>> _loggerMock;

        public PostTests()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _loggerMock = new Mock<ILogger<UserController>>();
        }
        
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task WhenUsernameIsNull_ShouldThrowException(string newUserName)
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new BankContext(options);
            context.Users.Add(new User() {Username = "Admin"});
            context.SaveChangesAsync();

            var sut = new UserController(context, _httpClientFactoryMock.Object, _loggerMock.Object);

            var exception = await Record.ExceptionAsync(async () => await sut.Post("Admin", new UserDTO(){Username = newUserName}));
            Assert.NotNull(exception);
            Assert.IsType<InvalidInputException>(exception);
            Assert.Contains("New user's username cannot be empty.", exception.Message);
        } 
        
        [Fact]
        public async Task WhenUsernameTooLong_ShouldThrowException()
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new BankContext(options);
            context.Users.Add(new User() {Username = "Admin"});
            context.SaveChangesAsync();

            var sut = new UserController(context, _httpClientFactoryMock.Object, _loggerMock.Object);

            var exception = await Record.ExceptionAsync(async () => await sut.Post("Admin", new UserDTO(){Username = "verylongname"}));
            Assert.NotNull(exception);
            Assert.IsType<InvalidInputException>(exception);
            Assert.Contains("New user username's maximum length is 10", exception.Message);
        } 

        [Fact]
        public async Task WhenUserNotFound_ShouldThrowException()
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new BankContext(options);

            var sut = new UserController(context, _httpClientFactoryMock.Object, _loggerMock.Object);

            var exception = await Record.ExceptionAsync(async () => await sut.Post("username", new UserDTO(){Username = "new user"}));
            Assert.NotNull(exception);
            Assert.IsType<ItemNotFoundException>(exception);
            Assert.Equal("User not found.", exception.Message);
        } 
        
        [Fact]
        public async Task WhenUserIsNotAdmin_ShouldThrowException()
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new BankContext(options);
            context.Users.Add(new User() {Username = "username"});
            context.SaveChangesAsync();

            var sut = new UserController(context, _httpClientFactoryMock.Object, _loggerMock.Object);

            var exception = await Record.ExceptionAsync(async () => await sut.Post("username", new UserDTO(){Username = "new user"}));
            Assert.NotNull(exception);
            Assert.IsType<InvalidPermissionException>(exception);
            Assert.Equal("Only admin users can create new users.", exception.Message);
        }
        
        
        [Fact]
        public async Task WhenUsernameAlreadyExist_ShouldThrowException()
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new BankContext(options);
            context.Users.Add(new User() {Username = "Admin"});
            context.Users.Add(new User() {Username = "username"});
            context.SaveChangesAsync();

            var sut = new UserController(context, _httpClientFactoryMock.Object, _loggerMock.Object);

            var exception = await Record.ExceptionAsync(async () => await sut.Post("Admin", new UserDTO(){Username = "username"}));
            Assert.NotNull(exception);
            Assert.IsType<InvalidInputException>(exception);
            Assert.Equal("Username already exists.", exception.Message);
        }
        
                
        [Fact]
        public async Task WhenStatePostcodeCombinationInvalid_ShouldThrowException()
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new BankContext(options);
            context.Users.Add(new User() {Username = "Admin"});
            context.SaveChangesAsync();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Forbidden,
                });
            
            var client = new HttpClient(mockHttpMessageHandler.Object) {BaseAddress = new Uri("https://google.com")};
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var sut = new UserController(context, _httpClientFactoryMock.Object, _loggerMock.Object);

            ActionResult<UserDTO> result = null;
            var exception = await Record.ExceptionAsync(async () => result = await sut.Post("Admin", new UserDTO(){Username = "username"}));
            Assert.NotNull(exception);
            Assert.IsType<InvalidInputException>(exception);
            Assert.Equal("Invalid post code and state combination.", exception.Message);
        } 
        
        [Fact]
        public async Task WhenAllValid_ShouldReturnCreated()
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new BankContext(options);
            context.Users.Add(new User() {Username = "Admin"});
            await context.SaveChangesAsync();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                });

            var client = new HttpClient(mockHttpMessageHandler.Object) {BaseAddress = new Uri("https://google.com")};
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var sut = new UserController(context, _httpClientFactoryMock.Object, _loggerMock.Object);

            ActionResult<UserDTO> result = null;
            var exception = await Record.ExceptionAsync(async () => result = await sut.Post("Admin", new UserDTO(){Username = "username"}));
            var addResult = result.Result as CreatedResult;
            var dto = addResult.Value as UserDTO;
            Assert.Null(exception);
            Assert.NotNull(dto);
        } 
    }
}