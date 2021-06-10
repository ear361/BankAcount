using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BankAccountAPI.Controllers;
using BankAccountAPI.DTO;
using BankAccountAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;
using Xunit;

namespace BankAccountAPITest.Controllers
{
    public class UserController_PostTest
    {
        private Mock<IHttpClientFactory> HttpClientFactoryMock;

        public UserController_PostTest()
        {
            HttpClientFactoryMock = new Mock<IHttpClientFactory>();
        }

        [Fact]
        public async Task WhenUserNotFound_ShouldThrowException()
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(databaseName: "WhenUserNotFound_ShouldThrowException")
                .Options;

            var context = new BankContext(options);

            var sut = new UserController(context, HttpClientFactoryMock.Object);

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

            var sut = new UserController(context, HttpClientFactoryMock.Object);

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

            var sut = new UserController(context, HttpClientFactoryMock.Object);

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

            var sut = new UserController(context, HttpClientFactoryMock.Object);

            var exception = await Record.ExceptionAsync(async () => await sut.Post("Admin", new UserDTO(){Username = "username"}));
            Assert.NotNull(exception);
            Assert.Equal("Username already exists.", exception.Message);
        }
        
                
        [Fact]
        public async Task WhenStatePostcodeCombinationInvalid_ShouldReturnCreated()
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(databaseName: "WhenStatePostcodeCombinationInvalid_ShouldReturnCreated")
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
            
            var client = new HttpClient(mockHttpMessageHandler.Object);
            HttpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var sut = new UserController(context, HttpClientFactoryMock.Object);

            ActionResult<UserDTO> result = null;
            var exception = await Record.ExceptionAsync(async () => result = await sut.Post("Admin", new UserDTO(){Username = "username"}));
            Assert.NotNull(exception);
            Assert.Equal("Invalid post code and state combination.", exception.Message);
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

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                });
            
            var client = new HttpClient(mockHttpMessageHandler.Object);
            HttpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var sut = new UserController(context, HttpClientFactoryMock.Object);

            ActionResult<UserDTO> result = null;
            var exception = await Record.ExceptionAsync(async () => result = await sut.Post("Admin", new UserDTO(){Username = "username"}));
            Assert.Null(exception);
            Assert.NotNull(result);
        } 
    }
}