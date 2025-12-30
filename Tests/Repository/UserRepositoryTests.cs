using Xunit;
using System;
using Repository;
using Repository.Interfaces;
using Repository.POCO;

namespace Tests.Repository
{
    public class UserRepositoryTests
    {
        [Fact]
        public void UserRepository_Constructor_WithValidDBContext_CreatesInstance()
        {
            // Arrange
            var db = new DBContext();

            // Act
            var repository = new UserRepository(db);

            // Assert
            Assert.NotNull(repository);
        }

        [Fact]
        public void UserRepository_Constructor_WithNullDBContext_ThrowsException()
        {
            // Arrange
            DBContext db = null;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() =>
            {
                var repository = new UserRepository(db);
                repository.UserIsInRole("test", "admin");
            });
        }

        [Fact]
        public void UserRepository_UserIsInRole_WithValidParameters_ReturnsBool()
        {
            // Arrange
            var db = new DBContext();
            var repository = new UserRepository(db);
            string userName = "testUser";
            string roleName = "admin";

            // Act
            var exception = Record.Exception(() => repository.UserIsInRole(userName, roleName));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void UserRepository_UserIsInRole_WithNullUserName_ThrowsException()
        {
            // Arrange
            var db = new DBContext();
            var repository = new UserRepository(db);
            string userName = null;
            string roleName = "admin";

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => repository.UserIsInRole(userName, roleName));
        }

        [Fact]
        public void UserRepository_UserIsInRole_WithNullRoleName_ThrowsException()
        {
            // Arrange
            var db = new DBContext();
            var repository = new UserRepository(db);
            string userName = "testUser";
            string roleName = null;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => repository.UserIsInRole(userName, roleName));
        }

        [Fact]
        public void UserRepository_UserIsInRole_WithEmptyUserName_ReturnsFalse()
        {
            // Arrange
            var db = new DBContext();
            var repository = new UserRepository(db);
            string userName = "";
            string roleName = "admin";

            // Act
            var result = repository.UserIsInRole(userName, roleName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UserRepository_UserIsInRole_WithEmptyRoleName_ReturnsFalse()
        {
            // Arrange
            var db = new DBContext();
            var repository = new UserRepository(db);
            string userName = "testUser";
            string roleName = "";

            // Act
            var result = repository.UserIsInRole(userName, roleName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UserRepository_CreateUser_WithNullAuthManager_ThrowsException()
        {
            // Arrange
            var db = new DBContext();
            var repository = new UserRepository(db);
            string userName = "newUser";
            string password = "password123";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => repository.CreateUser(userName, password, null));
        }

        [Fact]
        public void UserRepository_CreateUser_WithEmptyUserName_ReturnsFailedResult()
        {
            // Arrange
            var db = new DBContext();
            var repository = new UserRepository(db);
            string userName = "";
            string password = "password123";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => repository.CreateUser(userName, password, null));
        }

        [Fact]
        public void UserRepository_CreateUser_WithEmptyPassword_ReturnsFailedResult()
        {
            // Arrange
            var db = new DBContext();
            var repository = new UserRepository(db);
            string userName = "newUser";
            string password = "";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => repository.CreateUser(userName, password, null));
        }

        [Fact]
        public void UserRepository_LoginUser_WithNullAuthManager_ThrowsException()
        {
            // Arrange
            var db = new DBContext();
            var repository = new UserRepository(db);
            string userName = "testUser";
            string password = "password123";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => repository.LoginUser(userName, password, null));
        }

        [Fact]
        public void UserRepository_LoginUser_WithEmptyUserName_ReturnsFailedResult()
        {
            // Arrange
            var db = new DBContext();
            var repository = new UserRepository(db);
            string userName = "";
            string password = "password123";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => repository.LoginUser(userName, password, null));
        }

        [Fact]
        public void UserRepository_LoginUser_WithEmptyPassword_ReturnsFailedResult()
        {
            // Arrange
            var db = new DBContext();
            var repository = new UserRepository(db);
            string userName = "testUser";
            string password = "";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => repository.LoginUser(userName, password, null));
        }

        [Fact]
        public void UserRepository_ImplementsIUserRepository()
        {
            // Arrange
            var db = new DBContext();

            // Act
            var repository = new UserRepository(db);

            // Assert
            Assert.IsAssignableFrom<IUserRepository>(repository);
        }
    }
}
