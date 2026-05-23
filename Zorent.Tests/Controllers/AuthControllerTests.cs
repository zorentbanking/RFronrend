using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

using Xunit;

using Zorent.API.Controllers;

using Zorent.BLL.DTOs.Auth;
using Zorent.BLL.Interfaces;

using Zorent.Common.Responses;

using Zorent.DAL.Data;

using Zorent.Domain.Entities;

namespace Zorent.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService>
            _authServiceMock;

        private readonly AuthController
            _controller;

        public AuthControllerTests()
        {
            _authServiceMock =
                new Mock<IAuthService>();

            _controller =
                new AuthController(
                    _authServiceMock.Object,
                    null
                );
        }

        // =====================================================
        // REGISTER 200
        // =====================================================

        [Fact]
        public async Task Register_Returns_200_OK()
        {
            var dto = new RegisterDto();

            _authServiceMock
                .Setup(x => x.Register(dto))
                .ReturnsAsync(
                    new ApiResponse
                    {
                        Success = true,
                        Message =
                            "Registered Successfully"
                    });

            var result =
                await _controller.Register(dto);

            Assert.IsType<OkObjectResult>(
                result);
        }

        // =====================================================
        // REGISTER 400
        // =====================================================

        [Fact]
        public async Task Register_Returns_400_BadRequest()
        {
            var dto = new RegisterDto();

            _authServiceMock
                .Setup(x => x.Register(dto))
                .ReturnsAsync(
                    new ApiResponse
                    {
                        Success = false,
                        Message =
                            "User already exists"
                    });

            var result =
                await _controller.Register(dto);

            Assert.IsType
                <BadRequestObjectResult>(
                    result);
        }

        // =====================================================
        // REGISTER 500
        // =====================================================

        [Fact]
        public async Task Register_Returns_500_Error()
        {
            var dto = new RegisterDto();

            _authServiceMock
                .Setup(x => x.Register(dto))
                .ThrowsAsync(
                    new Exception(
                        "Server Error"
                    ));

            var result =
                await _controller.Register(dto);

            var objectResult =
                Assert.IsType<ObjectResult>(
                    result);

            Assert.Equal(
                500,
                objectResult.StatusCode
            );
        }

        // =====================================================
        // LOGIN 200
        // =====================================================

        [Fact]
        public async Task Login_Returns_200_OK()
        {
            var dto =
                new LoginDto
                {
                    Username = "supra",
                    Password = "Test@123"
                };

            _authServiceMock
                .Setup(x => x.Login(dto))
                .ReturnsAsync(
                    new ApiResponse<AuthResponseDto>
                    {
                        Success = true,

                        Message =
                            "Login Success",

                        Data =
                            new AuthResponseDto
                            {
                                AccessToken =
                                    "token",

                                RefreshToken =
                                    "refresh",

                                Id = 1,

                                Username =
                                    "supra",

                                FullName =
                                    "Supra",

                                Email =
                                    "supra@gmail.com",

                                Phone =
                                    "9876543210",

                                Address =
                                    "Bangalore"
                            }
                    });

            var result =
                await _controller.Login(dto);

            Assert.IsType<OkObjectResult>(
                result);
        }

        // =====================================================
        // LOGIN 401
        // =====================================================

        [Fact]
        public async Task Login_Returns_401_Unauthorized()
        {
            var dto =
                new LoginDto
                {
                    Username = "supra",
                    Password = "wrong"
                };

            _authServiceMock
                .Setup(x => x.Login(dto))
                .ReturnsAsync(
                    new ApiResponse<AuthResponseDto>
                    {
                        Success = false,

                        Message =
                            "Invalid Credentials"
                    });

            var result =
                await _controller.Login(dto);

            Assert.IsType
                <UnauthorizedObjectResult>(
                    result);
        }

        // =====================================================
        // LOGIN 500
        // =====================================================

        [Fact]
        public async Task Login_Returns_500_Error()
        {
            var dto = new LoginDto();

            _authServiceMock
                .Setup(x => x.Login(dto))
                .ThrowsAsync(
                    new Exception(
                        "Server Error"
                    ));

            var result =
                await _controller.Login(dto);

            var objectResult =
                Assert.IsType<ObjectResult>(
                    result);

            Assert.Equal(
                500,
                objectResult.StatusCode
            );
        }

        // =====================================================
        // FORGOT PASSWORD 200
        // =====================================================

        [Fact]
        public async Task ForgotPassword_Returns_200_OK()
        {
            var dto =
                new ForgotPasswordDto
                {
                    Email =
                        "test@gmail.com"
                };

            _authServiceMock
                .Setup(x =>
                    x.ForgotPassword(dto))
                .ReturnsAsync(
                    "Reset Link Sent");

            var result =
                await _controller
                    .ForgotPassword(dto);

            Assert.IsType<OkObjectResult>(
                result);
        }

        // =====================================================
        // FORGOT PASSWORD 400
        // =====================================================

        [Fact]
        public async Task ForgotPassword_Returns_400_BadRequest()
        {
            var dto =
                new ForgotPasswordDto
                {
                    Email =
                        "wrong@gmail.com"
                };

            _authServiceMock
                .Setup(x =>
                    x.ForgotPassword(dto))
                .ReturnsAsync(
                    "Email is not registerd!!"
                );

            var result =
                await _controller
                    .ForgotPassword(dto);

            Assert.IsType
                <BadRequestObjectResult>(
                    result);
        }

        // =====================================================
        // FORGOT PASSWORD 500
        // =====================================================

        [Fact]
        public async Task ForgotPassword_Returns_500_Error()
        {
            var dto =
                new ForgotPasswordDto();

            _authServiceMock
                .Setup(x =>
                    x.ForgotPassword(dto))
                .ThrowsAsync(
                    new Exception(
                        "Server Error"
                    ));

            var result =
                await _controller
                    .ForgotPassword(dto);

            var objectResult =
                Assert.IsType<ObjectResult>(
                    result);

            Assert.Equal(
                500,
                objectResult.StatusCode
            );
        }

        // =====================================================
        // RESET PASSWORD 200
        // =====================================================

        [Fact]
        public async Task ResetPassword_Returns_200_OK()
        {
            var dto =
                new ResetPasswordDto();

            _authServiceMock
                .Setup(x =>
                    x.ResetPasswordAsync(dto))
                .ReturnsAsync(
                    "Password reset successful"
                );

            var result =
                await _controller
                    .ResetPassword(dto);

            Assert.IsType<OkObjectResult>(
                result);
        }

        // =====================================================
        // RESET PASSWORD 400
        // =====================================================

        [Fact]
        public async Task ResetPassword_Returns_400_BadRequest()
        {
            var dto =
                new ResetPasswordDto();

            _authServiceMock
                .Setup(x =>
                    x.ResetPasswordAsync(dto))
                .ReturnsAsync(
                    "Invalid Token"
                );

            var result =
                await _controller
                    .ResetPassword(dto);

            Assert.IsType
                <BadRequestObjectResult>(
                    result);
        }

        // =====================================================
        // RESET PASSWORD 500
        // =====================================================

        [Fact]
        public async Task ResetPassword_Returns_500_Error()
        {
            var dto =
                new ResetPasswordDto();

            _authServiceMock
                .Setup(x =>
                    x.ResetPasswordAsync(dto))
                .ThrowsAsync(
                    new Exception(
                        "Server Error"
                    ));

            var result =
                await _controller
                    .ResetPassword(dto);

            var objectResult =
                Assert.IsType<ObjectResult>(
                    result);

            Assert.Equal(
                500,
                objectResult.StatusCode
            );
        }

        // =====================================================
        // VALIDATE ACCOUNT 200
        // =====================================================

        [Fact]
        public async Task ValidateAccount_Returns_200_OK()
        {
            var options =
                new DbContextOptionsBuilder
                    <ApplicationDbContext>()
                .UseInMemoryDatabase(
                    databaseName:
                        "ValidateAccountDb"
                )
                .Options;

            using var context =
                new ApplicationDbContext(
                    options
                );

            context.Accounts.Add(
                new Account
                {
                    AccountNumber =
                        "1234567890"
                });

            await context
                .SaveChangesAsync();

            var controller =
                new AuthController(
                    _authServiceMock.Object,
                    context
                );

            var result =
                await controller
                    .ValidateAccount(
                        "1234567890"
                    );

            Assert.IsType<OkObjectResult>(
                result);
        }

        // =====================================================
        // VALIDATE ACCOUNT EMPTY 400
        // =====================================================

        [Fact]
        public async Task ValidateAccount_Empty_Returns_400()
        {
            var options =
                new DbContextOptionsBuilder
                    <ApplicationDbContext>()
                .UseInMemoryDatabase(
                    databaseName:
                        "ValidateAccountDb2"
                )
                .Options;

            using var context =
                new ApplicationDbContext(
                    options
                );

            var controller =
                new AuthController(
                    _authServiceMock.Object,
                    context
                );

            var result =
                await controller
                    .ValidateAccount("");

            Assert.IsType
                <BadRequestObjectResult>(
                    result);
        }

        // =====================================================
        // VALIDATE ACCOUNT LENGTH 400
        // =====================================================

        [Fact]
        public async Task ValidateAccount_Length_Returns_400()
        {
            var options =
                new DbContextOptionsBuilder
                    <ApplicationDbContext>()
                .UseInMemoryDatabase(
                    databaseName:
                        "ValidateAccountDb3"
                )
                .Options;

            using var context =
                new ApplicationDbContext(
                    options
                );

            var controller =
                new AuthController(
                    _authServiceMock.Object,
                    context
                );

            var result =
                await controller
                    .ValidateAccount(
                        "123"
                    );

            Assert.IsType
                <BadRequestObjectResult>(
                    result);
        }

        // =====================================================
        // VALIDATE ACCOUNT CHARACTER 400
        // =====================================================

        [Fact]
        public async Task ValidateAccount_Character_Returns_400()
        {
            var options =
                new DbContextOptionsBuilder
                    <ApplicationDbContext>()
                .UseInMemoryDatabase(
                    databaseName:
                        "ValidateAccountDb4"
                )
                .Options;

            using var context =
                new ApplicationDbContext(
                    options
                );

            var controller =
                new AuthController(
                    _authServiceMock.Object,
                    context
                );

            var result =
                await controller
                    .ValidateAccount(
                        "12345abcd1"
                    );

            Assert.IsType
                <BadRequestObjectResult>(
                    result);
        }

        // =====================================================
        // VALIDATE ACCOUNT INVALID
        // =====================================================

        [Fact]
        public async Task
        ValidateAccount_Invalid_Returns_200()
        {
            var options =
                new DbContextOptionsBuilder
                    <ApplicationDbContext>()
                .UseInMemoryDatabase(
                    databaseName:
                        "ValidateAccountDb5"
                )
                .Options;

            using var context =
                new ApplicationDbContext(
                    options
                );

            var controller =
                new AuthController(
                    _authServiceMock.Object,
                    context
                );

            var result =
                await controller
                    .ValidateAccount(
                        "9999999999"
                    );

            Assert.IsType<OkObjectResult>(
                result);
        }

        // =====================================================
        // VALIDATE RESET TOKEN 200
        // =====================================================

        [Fact]
        public async Task
        ValidateResetToken_Returns_200_OK()
        {
            var options =
                new DbContextOptionsBuilder
                    <ApplicationDbContext>()
                .UseInMemoryDatabase(
                    databaseName:
                        "ValidateTokenDb"
                )
                .Options;

            using var context =
                new ApplicationDbContext(
                    options
                );

            context.Users.Add(
                new User
                {
                    ResetToken =
                        "validtoken",

                    ResetTokenExpiry =
                        DateTime.UtcNow
                            .AddMinutes(30)
                });

            await context
                .SaveChangesAsync();

            var controller =
                new AuthController(
                    _authServiceMock.Object,
                    context
                );

            var result =
                await controller
                    .ValidateResetToken(
                        "validtoken"
                    );

            Assert.IsType<OkObjectResult>(
                result);
        }

        // =====================================================
        // VALIDATE RESET TOKEN INVALID
        // =====================================================

        [Fact]
        public async Task
        ValidateResetToken_Invalid_Returns_400()
        {
            var options =
                new DbContextOptionsBuilder
                    <ApplicationDbContext>()
                .UseInMemoryDatabase(
                    databaseName:
                        "ValidateTokenDb2"
                )
                .Options;

            using var context =
                new ApplicationDbContext(
                    options
                );

            var controller =
                new AuthController(
                    _authServiceMock.Object,
                    context
                );

            var result =
                await controller
                    .ValidateResetToken(
                        "wrongtoken"
                    );

            Assert.IsType
                <BadRequestObjectResult>(
                    result);
        }

        // =====================================================
        // VALIDATE RESET TOKEN EXPIRED
        // =====================================================

        [Fact]
        public async Task
        ValidateResetToken_Expired_Returns_400()
        {
            var options =
                new DbContextOptionsBuilder
                    <ApplicationDbContext>()
                .UseInMemoryDatabase(
                    databaseName:
                        "ValidateTokenDb3"
                )
                .Options;

            using var context =
                new ApplicationDbContext(
                    options
                );

            context.Users.Add(
                new User
                {
                    ResetToken =
                        "expiredtoken",

                    ResetTokenExpiry =
                        DateTime.UtcNow
                            .AddMinutes(-5)
                });

            await context
                .SaveChangesAsync();

            var controller =
                new AuthController(
                    _authServiceMock.Object,
                    context
                );

            var result =
                await controller
                    .ValidateResetToken(
                        "expiredtoken"
                    );

            Assert.IsType
                <BadRequestObjectResult>(
                    result);
        }

        // =====================================================
        // VALIDATE RESET TOKEN 500
        // =====================================================

        [Fact]
        public async Task
        ValidateResetToken_Returns_500_Error()
        {
            var options =
                new DbContextOptionsBuilder
                    <ApplicationDbContext>()
                .UseInMemoryDatabase(
                    databaseName:
                        "ValidateTokenDb4"
                )
                .Options;

            var context =
                new ApplicationDbContext(
                    options
                );

            context.Dispose();

            var controller =
                new AuthController(
                    _authServiceMock.Object,
                    context
                );

            var result =
                await controller
                    .ValidateResetToken(
                        "token"
                    );

            var objectResult =
                Assert.IsType<ObjectResult>(
                    result);

            Assert.Equal(
                500,
                objectResult.StatusCode
            );
        }
    }
}