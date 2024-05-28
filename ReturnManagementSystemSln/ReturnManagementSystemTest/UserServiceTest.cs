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
            var options = new DbContextOptionsBuilder<ReturnManagementSystemContext>().UseInMemoryDatabase("DummyDB").Options;
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
                Username = "test",
                Password = "Test@123"
            };

            var user = await _userService.Register(registerUserDTO);

            Assert.IsNotNull(user);
        }

        [Test]
        public async Task UserLoginFailTest()
        {
            UserLoginDTO userLoginDTO = new UserLoginDTO()
            {
                UserId = 1,
                Password = "Test@123"
            };

            Assert.ThrowsAsync<UnauthorizedUserException>(async ()=> await _userService.Login(userLoginDTO));
        }
    }
}
