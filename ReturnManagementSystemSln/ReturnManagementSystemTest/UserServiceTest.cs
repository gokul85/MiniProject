using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models;
using ReturnManagementSystem.Models.DTOs;
using ReturnManagementSystem.Repositories;
using ReturnManagementSystem.Services;
using ReturnManagementSystem.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReturnManagementSystemTest
{
    public class UserServiceTest
    {
        IUserService _userService;
        ITokenService _tokenService;
        IRepository<int,User> _userRepository;
        IRepository<int, UserDetail> _userdetailsRepository;
        ReturnManagementSystemContext context;
        [SetUp]
        public void Setup()
        {
            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("This is the dummy key which has to be a bit long for the 512. which should be even more longer for the passing");
            Mock<IConfigurationSection> configTokenSection = new Mock<IConfigurationSection>();
            configTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(configTokenSection.Object);
            _tokenService = new TokenService(mockConfig.Object);
            var options = new DbContextOptionsBuilder<ReturnManagementSystemContext>().UseInMemoryDatabase("DummyDB2").Options;
            context = new ReturnManagementSystemContext(options);
            _userRepository = new UserRepository(context);
            _userdetailsRepository = new UserDetailRepository(context);
            _userService = new UserService(_userRepository,_userdetailsRepository,_tokenService);
        }

        [Test]
        public async Task RegisterUserTestSuccess()
        {
            RegisterUserDTO registerUserDTO = new RegisterUserDTO()
            {
                Name = "Test",
                Phone = "9876543210",
                Email = "test@gmail.com",
                Address = "Address",
                Username = "testuser",
                Password = "Test@123"
            };

            var user = await _userService.Register(registerUserDTO);

            Assert.IsNotNull(user);
            Assert.That(user.Username, Is.EqualTo("testuser"));
        }

        [Test]
        public async Task RegisterUserTestUsernameExists()
        {
            var registerUserDTO = new RegisterUserDTO
            {
                Name = "Test User",
                Phone = "9876543210",
                Email = "test@gmail.com",
                Address = "Test Address",
                Username = "testuser",
                Password = "Test@123"
            };

            Assert.ThrowsAsync<UsernameAlreadyExistException>(async () => await _userService.Register(registerUserDTO));
        }

        [Test]
        public async Task RegisterUserTestEmailExists()
        {
            var registerUserDTO1 = new RegisterUserDTO
            {
                Name = "Test User",
                Phone = "9876543210",
                Email = "test2@gmail.com",
                Address = "Test Address",
                Username = "test",
                Password = "Test@123"
            };

            var registerUserDTO2 = new RegisterUserDTO
            {
                Name = "Test User",
                Phone = "9876543210",
                Email = "test2@gmail.com",
                Address = "Test Address",
                Username = "test2",
                Password = "Test@123"
            };
            await _userService.Register(registerUserDTO1);

            Assert.ThrowsAsync<UserAlreadyExistException>(async () => await _userService.Register(registerUserDTO2));
        }

        [Test]
        public async Task UserLoginInactiveUser()
        {
            UserLoginDTO userLoginDTO = new UserLoginDTO()
            {
                Username = "test",
                Password = "Test@123"
            };


            Assert.ThrowsAsync<UserNotActiveException>(async () => await _userService.Login(userLoginDTO));
        }

        [TestCase("test2","pass")]
        [TestCase("test","pass")]
        public async Task UserLoginTestFail(string uname, string pass)
        {
            var userLoginDTO = new UserLoginDTO
            {
                Username = uname,
                Password = pass
            };

            Assert.ThrowsAsync<UnauthorizedUserException>(async () => await _userService.Login(userLoginDTO));
        }

        [Test]
        public async Task AddUserUpdateStatusLoginTest()
        {
            var registerUserDTO = new RegisterUserDTO
            {
                Name = "Test User",
                Phone = "9876543210",
                Email = "testuser@gmail.com",
                Address = "TestAddress",
                Username = "usertest",
                Password = "Test@123"
            };

            var registeredUser = await _userService.Register(registerUserDTO);

            var updateUserStatusDTO = new UserUpdateStatusDTO
            {
                UserId = registeredUser.UserId,
                Status = "Active"
            };

            var updateResult = await _userService.UpdateUserStatus(updateUserStatusDTO);
            Assert.That(updateResult, Is.EqualTo("User Status Successfully Updated"));

            var userLoginDTO = new UserLoginDTO
            {
                Username = "usertest",
                Password = "Test@123"
            };

            var loginResult = await _userService.Login(userLoginDTO);
            Assert.IsNotNull(loginResult);
            Assert.That(loginResult.Username, Is.EqualTo("usertest"));
        }

        [Test]
        public void UpdateUserStatusTestNoUserFound()
        {
            var updateUserStatusDTO = new UserUpdateStatusDTO
            {
                UserId = 999,
                Status = "Active"
            };

            Assert.ThrowsAsync<NoUserFoundException>(async () => await _userService.UpdateUserStatus(updateUserStatusDTO));
        }

    }
}
