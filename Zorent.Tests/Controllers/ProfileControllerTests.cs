using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

using Xunit;

using Zorent.API.Controllers;

using Zorent.BLL.DTOs;

using Zorent.DAL.Data;

using Zorent.Domain.Entities;

namespace Zorent.Tests.Controllers
{
    public class ProfileControllerTests
    {
        private readonly ApplicationDbContext
            _context;

        private readonly ProfileController
            _controller;

        public ProfileControllerTests()
        {
            var options =
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(
                    databaseName:
                        Guid.NewGuid().ToString())
                .Options;

            _context =
                new ApplicationDbContext(options);

            var user =
                new User
                {
                    Id = 1,

                    Username = "testuser",

                    FullName = "Test User",

                    Email = "test@gmail.com",

                    Phone = "9999999999",

                    Address = "Bangalore",

                    DOB = new DateTime(
                        2000,
                        1,
                        1),

                    PasswordHash =
                        BCrypt.Net.BCrypt
                        .HashPassword("Old123")
                };

            _context.Users.Add(user);

            _context.SaveChanges();

            _controller =
                new ProfileController(_context);

            var claims =
                new List<Claim>
                {
                    new Claim(
                        ClaimTypes.Name,
                        "testuser")
                };

            var identity =
                new ClaimsIdentity(
                    claims);

            var principal =
                new ClaimsPrincipal(
                    identity);

            _controller.ControllerContext =
                new ControllerContext
                {
                    HttpContext =
                        new DefaultHttpContext
                        {
                            User = principal
                        }
                };
        }

        // =====================================================
        // GET PROFILE
        // =====================================================

        [Fact]

        public async Task
            GetProfile_Returns_200_OK()
        {
            var result =
                await _controller
                    .GetProfile();

            Assert.IsType<OkObjectResult>(
                result);
        }

        [Fact]

        public async Task
            GetProfile_Returns_404_NotFound()
        {
            var controller =
                new ProfileController(
                    _context);

            var claims =
                new List<Claim>
                {
                    new Claim(
                        ClaimTypes.Name,
                        "wronguser")
                };

            var identity =
                new ClaimsIdentity(
                    claims);

            var principal =
                new ClaimsPrincipal(
                    identity);

            controller.ControllerContext =
                new ControllerContext
                {
                    HttpContext =
                        new DefaultHttpContext
                        {
                            User = principal
                        }
                };

            var result =
                await controller
                    .GetProfile();

            Assert.IsType<NotFoundObjectResult>(
                result);
        }
        // =====================================================
        // GET PROFILE - 500
        // =====================================================

        [Fact]
        public async Task
        GetProfile_Returns_500_Error()
        {
            _context.Dispose();

            var result =
                await _controller
                    .GetProfile();

            var objectResult =
                Assert.IsType<ObjectResult>(
                    result);

            Assert.Equal(
                500,
                objectResult.StatusCode);
        }
        // =====================================================
        // UPDATE PROFILE
        // =====================================================

        [Fact]

        public async Task
            UpdateProfile_Returns_200_OK()
        {
            var dto =
                new UpdateProfileDto
                {
                    FullName = "Updated",

                    Phone = "8888888888",

                    Address = "Hyderabad",

                    DateOfBirth =
                        new DateTime(
                            1999,
                            1,
                            1)
                };

            var result =
                await _controller
                    .UpdateProfile(dto);

            Assert.IsType<OkObjectResult>(
                result);
        }

        [Fact]

        public async Task
            UpdateProfile_Returns_404_NotFound()
        {
            var controller =
                new ProfileController(
                    _context);

            var claims =
                new List<Claim>
                {
                    new Claim(
                        ClaimTypes.Name,
                        "wronguser")
                };

            var identity =
                new ClaimsIdentity(
                    claims);

            var principal =
                new ClaimsPrincipal(
                    identity);

            controller.ControllerContext =
                new ControllerContext
                {
                    HttpContext =
                        new DefaultHttpContext
                        {
                            User = principal
                        }
                };

            var dto =
                new UpdateProfileDto();

            var result =
                await controller
                    .UpdateProfile(dto);

            Assert.IsType<NotFoundObjectResult>(
                result);
        }

        // =====================================================
        // UPDATE PROFILE - 500
        // =====================================================

        [Fact]
        public async Task
        UpdateProfile_Returns_500_Error()
        {
            _context.Dispose();

            var dto =
                new UpdateProfileDto
                {
                    FullName = "Updated"
                };

            var result =
                await _controller
                    .UpdateProfile(dto);

            var objectResult =
                Assert.IsType<ObjectResult>(
                    result);

            Assert.Equal(
                500,
                objectResult.StatusCode);
        }

        // =====================================================
        // CHANGE PASSWORD
        // =====================================================

        [Fact]

        public async Task
            ChangePassword_Returns_200_OK()
        {
            var dto =
                new ChangePasswordDto
                {
                    CurrentPassword =
                        "Old123",

                    NewPassword =
                        "New123"
                };

            var result =
                await _controller
                    .ChangePassword(dto);

            Assert.IsType<OkObjectResult>(
                result);
        }

        [Fact]

        public async Task
            ChangePassword_Returns_400_BadRequest()
        {
            var dto =
                new ChangePasswordDto
                {
                    CurrentPassword =
                        "WrongPassword",

                    NewPassword =
                        "New123"
                };

            var result =
                await _controller
                    .ChangePassword(dto);

            Assert.IsType<BadRequestObjectResult>(
                result);
        }

        [Fact]

        public async Task
            ChangePassword_Returns_404_NotFound()
        {
            var controller =
                new ProfileController(
                    _context);

            var claims =
                new List<Claim>
                {
                    new Claim(
                        ClaimTypes.Name,
                        "wronguser")
                };

            var identity =
                new ClaimsIdentity(
                    claims);

            var principal =
                new ClaimsPrincipal(
                    identity);

            controller.ControllerContext =
                new ControllerContext
                {
                    HttpContext =
                        new DefaultHttpContext
                        {
                            User = principal
                        }
                };

            var dto =
                new ChangePasswordDto
                {
                    CurrentPassword =
                        "Old123",

                    NewPassword =
                        "New123"
                };

            var result =
                await controller
                    .ChangePassword(dto);

            Assert.IsType<NotFoundObjectResult>(
                result);
        }
        // =====================================================
        // CHANGE PASSWORD - 500
        // =====================================================

        [Fact]
        public async Task
        ChangePassword_Returns_500_Error()
        {
            _context.Dispose();

            var dto =
                new ChangePasswordDto
                {
                    CurrentPassword =
                        "Old123",

                    NewPassword =
                        "New123"
                };

            var result =
                await _controller
                    .ChangePassword(dto);

            var objectResult =
                Assert.IsType<ObjectResult>(
                    result);

            Assert.Equal(
                500,
                objectResult.StatusCode);
        }

        // =====================================================
        // CHANGE PASSWORD - EMPTY PASSWORD
        // =====================================================

        [Fact]
        public async Task
        ChangePassword_Returns_400_EmptyPassword()
        {
            var dto =
                new ChangePasswordDto
                {
                    CurrentPassword = "",
                    NewPassword = ""
                };

            var result =
                await _controller
                    .ChangePassword(dto);

            Assert.IsType<BadRequestObjectResult>(
                result);
        }
    }
}