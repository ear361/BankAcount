using BankAccountAPI.Helpers;
using Xunit;

namespace BankAccountAPITest.Helpers.ValidationHelperTests
{
    public class ValidateUsernameTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void WhenUsernameIsEmptyOrNull_ShouldThrowError(string username)
        {
            var exception = Record.Exception(username.ValidateUsername);
            Assert.NotNull(exception);
            Assert.Contains("Username cannot be empty", exception.Message);
        }

        [Theory]
        [InlineData("ab")]
        [InlineData("arb  lin")]
        public void WhenUsernameIsValid_ShouldNotThrowError(string username)
        {
            var exception = Record.Exception(username.ValidateUsername);
            Assert.Null(exception);
        }
        
        [Theory]
        [InlineData("abababababababababab")]
        [InlineData("arb          lin")]
        public void WhenUsernameTooLong_ShouldThrowError(string username)
        {
            var exception = Record.Exception(username.ValidateUsername);
            Assert.NotNull(exception);
            Assert.Contains("Username's maximum length is 10", exception.Message);
        }
    }
}