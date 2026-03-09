using LoginNet.Domain.Entities;
using Xunit;

namespace LoginNet.Tests.UnitTests.Domain
{
    public class UserTests
    {
        [Fact]
        public void Create_WithValidData_ShouldCreateUser()
        {
            // Arrange
            var username = "testuser";
            var roleId = 1;
            var passwordHash = "hash";

            // Act
            var user = User.Create(username, roleId, passwordHash);

            // Assert
            Assert.Equal(username, user.Username);
            Assert.Equal(roleId, user.RoleId);
            Assert.Equal(passwordHash, user.PasswordHash);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithInvalidUsername_ShouldThrowArgumentException(string? invalidUsername)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => User.Create(invalidUsername!, 1, "hash"));
        }
    }
}
