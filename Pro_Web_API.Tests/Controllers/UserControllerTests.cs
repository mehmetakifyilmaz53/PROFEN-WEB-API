using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pro_Web_API.Business.Abstract;
using Pro_Web_API.Business.Concrete;
using Pro_Web_API.Core.DTO;
using Pro_Web_API.Core.Entities;
using Pro_Web_API.WebAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pro_Web_API.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UserController(_userServiceMock.Object);
        }

        private void MockUserWithRole(int role, string userId = "1")
        {
            string roleName = role switch
            {
                (int)UserRole.Admin => UserRole.Admin.ToString(),
                (int)UserRole.Manager => UserRole.Manager.ToString(),
                (int)UserRole.Viewer => UserRole.Viewer.ToString(),
                _ => "Unknown"
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, roleName)
        }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task Register_ShouldReturnForbidden_WhenUserIsManagerOrViewer()
        {
            var userDto = new RegisterUserDto { UserName = "TestUser", PasswordHash = "TestPass", Role = UserRole.Admin };
            MockUserWithRole(1);
            var result = await _controller.Register(userDto);
            Assert.IsType<ForbidResult>(result);

            MockUserWithRole(2);
            result = await _controller.Register(userDto);
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnForbidden_WhenUserIsNotAdmin()
        {
            MockUserWithRole(1);
            var result = await _controller.GetUserById(1);
            Assert.IsType<ForbidResult>(result);

            MockUserWithRole(2); 
            result = await _controller.GetUserById(1);
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnOk_WhenUserIsAdmin()
        {
            MockUserWithRole(0); 
            var user = new User { Id = 1, user_Name = "ExistingUser" };

            _userServiceMock.Setup(service => service.GetUserByIdAsync(1))
                .ReturnsAsync(new ServiceResponse<User> { Success = true, Data = user });

            var result = await _controller.GetUserById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(user, ((ServiceResponse<User>)okResult.Value).Data);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnForbidden_WhenUserIsNotAdmin()
        {
            MockUserWithRole(2); 
            var result = await _controller.DeleteUser(1);
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnOk_WhenUserIsAdmin()
        {
            MockUserWithRole(0);
            _userServiceMock.Setup(service => service.DeleteUserAsync(1))
                .ReturnsAsync(new ServiceResponse<bool> { Success = true });

            var result = await _controller.DeleteUser(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnForbidden_WhenUserIsNotAdmin()
        {
            MockUserWithRole(1);
            var updateUserDto = new UpdateUserDto { UserName = "UpdatedUser" };

            var result = await _controller.UpdateUser(1, updateUserDto);
            Assert.IsType<ForbidResult>(result);

            MockUserWithRole(2);
            result = await _controller.UpdateUser(1, updateUserDto);
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnOk_WhenUserIsAdmin()
        {
            MockUserWithRole(0); 
            var updateUserDto = new UpdateUserDto { UserName = "UpdatedUser" };

            _userServiceMock.Setup(service => service.UpdateUserAsync(1, updateUserDto))
                .ReturnsAsync(new ServiceResponse<bool> { Success = true });

            var result = await _controller.UpdateUser(1, updateUserDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }
    }
}
