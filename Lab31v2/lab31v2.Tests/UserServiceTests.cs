using Moq;
using Xunit;
using lab31v2;

namespace lab31v2.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            // Ініціалізуємо моби для інтерфейсів
            _userRepoMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            
            // Передаємо моби в сервіс
            _userService = new UserService(_userRepoMock.Object, _passwordHasherMock.Object);
        }

        // 1. Тест успішної реєстрації
        [Fact]
        public void Register_ValidData_ReturnsTrueAndAddsUser()
        {
            // Arrange
            _userRepoMock.Setup(repo => repo.GetByUsername("testuser")).Returns((User)null);
            _passwordHasherMock.Setup(hash => hash.HashPassword("password123")).Returns("hashed_pwd");

            // Act
            var result = _userService.Register("testuser", "password123");

            // Assert
            Assert.True(result);
            // Перевіряємо, що метод Add був викликаний рівно 1 раз із будь-яким об'єктом User
            _userRepoMock.Verify(repo => repo.Add(It.IsAny<User>()), Times.Once);
        }

        // 2. Тест реєстрації, коли користувач вже існує
        [Fact]
        public void Register_ExistingUser_ReturnsFalseAndDoesNotAddUser()
        {
            // Arrange
            var existingUser = new User { Username = "testuser" };
            _userRepoMock.Setup(repo => repo.GetByUsername("testuser")).Returns(existingUser);

            // Act
            var result = _userService.Register("testuser", "password123");

            // Assert
            Assert.False(result);
            // Перевіряємо, що Add ніколи не викликався
            _userRepoMock.Verify(repo => repo.Add(It.IsAny<User>()), Times.Never);
        }

        // 3. Тест реєстрації з пустим ім'ям
        [Theory]
        [InlineData("", "pass")]
        [InlineData(" ", "pass")]
        [InlineData(null, "pass")]
        public void Register_InvalidUsername_ReturnsFalse(string username, string password)
        {
            // Act
            var result = _userService.Register(username, password);

            // Assert
            Assert.False(result);
            _userRepoMock.Verify(repo => repo.Add(It.IsAny<User>()), Times.Never);
        }

        // 4. Тест успішного логіну
        [Fact]
        public void Login_ValidCredentials_ReturnsTrue()
        {
            // Arrange
            var user = new User { Username = "user", PasswordHash = "hash" };
            _userRepoMock.Setup(repo => repo.GetByUsername("user")).Returns(user);
            _passwordHasherMock.Setup(h => h.VerifyPassword("pass", "hash")).Returns(true);

            // Act
            var result = _userService.Login("user", "pass");

            // Assert
            Assert.True(result);
        }

        // 5. Тест логіну з неправильним паролем
        [Fact]
        public void Login_InvalidPassword_ReturnsFalse()
        {
            // Arrange
            var user = new User { Username = "user", PasswordHash = "hash" };
            _userRepoMock.Setup(repo => repo.GetByUsername("user")).Returns(user);
            _passwordHasherMock.Setup(h => h.VerifyPassword("wrongpass", "hash")).Returns(false);

            // Act
            var result = _userService.Login("user", "wrongpass");

            // Assert
            Assert.False(result);
        }

        // 6. Тест логіну, якщо користувача не знайдено
        [Fact]
        public void Login_UserNotFound_ReturnsFalse()
        {
            // Arrange
            _userRepoMock.Setup(repo => repo.GetByUsername("unknown")).Returns((User)null);

            // Act
            var result = _userService.Login("unknown", "pass");

            // Assert
            Assert.False(result);
            // Перевіряємо, що перевірка пароля навіть не викликалась
            _passwordHasherMock.Verify(h => h.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        // 7. Тест успішної зміни пароля
        [Fact]
        public void ChangePassword_ValidData_ReturnsTrueAndUpdatesUser()
        {
            // Arrange
            var user = new User { Username = "user", PasswordHash = "old_hash" };
            _userRepoMock.Setup(repo => repo.GetByUsername("user")).Returns(user);
            _passwordHasherMock.Setup(h => h.VerifyPassword("oldPass", "old_hash")).Returns(true);
            _passwordHasherMock.Setup(h => h.HashPassword("newPass")).Returns("new_hash");

            // Act
            var result = _userService.ChangePassword("user", "oldPass", "newPass");

            // Assert
            Assert.True(result);
            Assert.Equal("new_hash", user.PasswordHash);
            _userRepoMock.Verify(repo => repo.Update(user), Times.Once);
        }

        // 8. Тест зміни пароля з неправильним старим паролем
        [Fact]
        public void ChangePassword_WrongOldPassword_ReturnsFalseAndDoesNotUpdate()
        {
            // Arrange
            var user = new User { Username = "user", PasswordHash = "old_hash" };
            _userRepoMock.Setup(repo => repo.GetByUsername("user")).Returns(user);
            _passwordHasherMock.Setup(h => h.VerifyPassword("wrongOldPass", "old_hash")).Returns(false);

            // Act
            var result = _userService.ChangePassword("user", "wrongOldPass", "newPass");

            // Assert
            Assert.False(result);
            _userRepoMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
        }
    }
}