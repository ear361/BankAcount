using BankAccountAPI.Helpers;
using Xunit;

namespace BankAccountAPITest.Helpers
{
    public class ValidationHelperTest_ValidateUsername
    {
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void WhenUsernameIsEmptyOrNull_ShouldThrowError(string username)
        {
            var exception = Record.Exception(username.ValidateUsername);
            Assert.NotNull(exception);
            Assert.Equal("Username cannot be empty", exception.Message);
        }

        [Theory]
        [InlineData("ab")]
        [InlineData("arb  lin")]
        public void WhenUsernameIsValid_ShouldNotThrowError(string username)
        {
            var exception = Record.Exception(username.ValidateUsername);
            Assert.Null(exception);
        }
    }
}