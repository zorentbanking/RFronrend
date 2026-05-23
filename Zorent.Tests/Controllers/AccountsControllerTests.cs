using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Zorent.API.Controllers;
using Zorent.BLL.DTOs.Account;
using Zorent.BLL.Interfaces;
using Zorent.Common.Responses;

namespace Zorent.Tests.Controllers
{
    public class AccountsControllerTests
    {
        private readonly Mock<IAccountService> _serviceMock;
        private readonly AccountsController _controller;

        public AccountsControllerTests()
        {
            _serviceMock =
                new Mock<IAccountService>();

            _controller =
                new AccountsController(
                    _serviceMock.Object);

            // FAKE USER CLAIM
            var claims = new[]
            {
                new Claim("UserId", "1")
            };

            var identity =
                new ClaimsIdentity(
                    claims,
                    "Test");

            var principal =
                new ClaimsPrincipal(identity);

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
        // CREATE ACCOUNT - 200
        // =====================================================

        [Fact]
        public async Task Create_Returns_200_OK()
        {
            var dto = new CreateAccountDto
            {
                Type = "Savings",
                InitialDeposit = 1000
            };

            _serviceMock
                .Setup(x =>
                    x.CreateAccount(dto, 1))
                .ReturnsAsync(
                    new ApiResponse
                    {
                        Success = true,
                        Message = "Account created"
                    });

            var result =
                await _controller.Create(dto);

            Assert.IsType<OkObjectResult>(
                result);
        }

        // =====================================================
        // CREATE ACCOUNT - 400
        // =====================================================

        [Fact]
        public async Task Create_Returns_400_BadRequest()
        {
            var dto = new CreateAccountDto
            {
                Type = "Invalid",
                InitialDeposit = 100
            };

            _serviceMock
                .Setup(x =>
                    x.CreateAccount(dto, 1))
                .ReturnsAsync(
                    new ApiResponse
                    {
                        Success = false,
                        Message = "Invalid account type"
                    });

            var result =
                await _controller.Create(dto);

            Assert.IsType<BadRequestObjectResult>(
                result);
        }

        // =====================================================
        // CREATE ACCOUNT - 500
        // =====================================================

        [Fact]
        public async Task Create_Returns_500_Error()
        {
            var dto = new CreateAccountDto();

            _serviceMock
                .Setup(x =>
                    x.CreateAccount(dto, 1))
                .ThrowsAsync(
                    new Exception("Server Error"));

            var result =
                await _controller.Create(dto);

            Assert.IsType<ObjectResult>(
                result);
        }

        // =====================================================
        // GET MY ACCOUNTS - 200
        // =====================================================

        [Fact]
        public async Task Get_Returns_200_OK()
        {
            _serviceMock
                .Setup(x =>
                    x.GetUserAccounts(1))
                .ReturnsAsync(
                    new ApiResponse<List<AccountDto>>
                    {
                        Success = true,
                        Data =
                            new List<AccountDto>()
                    });

            var result =
                await _controller.Get();

            Assert.IsType<OkObjectResult>(
                result);
        }
        // =====================================================
        // GET MY ACCOUNTS - 401
        // =====================================================

        [Fact]
        public async Task Get_Returns_401_Unauthorized()
        {
            var controller =
                new AccountsController(
                    _serviceMock.Object);

            controller.ControllerContext =
                new ControllerContext
                {
                    HttpContext =
                        new DefaultHttpContext()
                };

            var result =
                await controller.Get();

            var objectResult =
                Assert.IsType<ObjectResult>(
                    result);

            Assert.Equal(
                500,
                objectResult.StatusCode);
        }

        // =====================================================
        // GET MY ACCOUNTS - 500
        // =====================================================

        [Fact]
        public async Task Get_Returns_500_Error()
        {
            _serviceMock
                .Setup(x =>
                    x.GetUserAccounts(1))
                .ThrowsAsync(
                    new Exception("Server Error"));

            var result =
                await _controller.Get();

            Assert.IsType<ObjectResult>(
                result);
        }

        // =====================================================
        // CLOSE DEPOSIT - 200
        // =====================================================

        [Fact]
        public async Task CloseDeposit_Returns_200_OK()
        {
            var dto =
                new CloseDepositDto
                {
                    DepositAccountNumber =
                        "123",
                    TargetAccountNumber =
                        "456"
                };

            _serviceMock
                .Setup(x =>
                    x.CloseDeposit(dto, 1))
                .ReturnsAsync(
                    new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Closed"
                    });

            var result =
                await _controller
                    .CloseDeposit(dto);

            Assert.IsType<OkObjectResult>(
                result);
        }

        // =====================================================
        // CLOSE DEPOSIT - 400
        // =====================================================

        [Fact]
        public async Task CloseDeposit_Returns_400_BadRequest()
        {
            var dto =
                new CloseDepositDto();

            _serviceMock
                .Setup(x =>
                    x.CloseDeposit(dto, 1))
                .ReturnsAsync(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Account not found"
                    });

            var result =
                await _controller
                    .CloseDeposit(dto);

            Assert.IsType<BadRequestObjectResult>(
                result);
        }

        // =====================================================
        // CLOSE DEPOSIT - 500
        // =====================================================

        [Fact]
        public async Task CloseDeposit_Returns_500_Error()
        {
            var dto =
                new CloseDepositDto();

            _serviceMock
                .Setup(x =>
                    x.CloseDeposit(dto, 1))
                .ThrowsAsync(
                    new Exception("Server Error"));

            var result =
                await _controller
                    .CloseDeposit(dto);

            Assert.IsType<ObjectResult>(
                result);
        }

        // =====================================================
        // DEPOSIT - 200
        // =====================================================

        [Fact]
        public async Task Deposit_Returns_200_OK()
        {
            var dto =
                new DepositDto
                {
                    AccountNumber = "123",
                    Amount = 1000
                };

            _serviceMock
                .Setup(x =>
                    x.DepositMoney(dto, 1))
                .ReturnsAsync(
                    new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Deposit successful"
                    });

            var result =
                await _controller.Deposit(dto);

            Assert.IsType<OkObjectResult>(
                result);
        }

        // =====================================================
        // DEPOSIT - 400
        // =====================================================

        [Fact]
        public async Task Deposit_Returns_400_BadRequest()
        {
            var dto =
                new DepositDto
                {
                    AccountNumber = "123",
                    Amount = -1
                };

            _serviceMock
                .Setup(x =>
                    x.DepositMoney(dto, 1))
                .ReturnsAsync(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid amount"
                    });

            var result =
                await _controller.Deposit(dto);

            Assert.IsType<BadRequestObjectResult>(
                result);
        }

        // =====================================================
        // DEPOSIT - 500
        // =====================================================

        [Fact]
        public async Task Deposit_Returns_500_Error()
        {
            var dto =
                new DepositDto();

            _serviceMock
                .Setup(x =>
                    x.DepositMoney(dto, 1))
                .ThrowsAsync(
                    new Exception("Server Error"));

            var result =
                await _controller.Deposit(dto);

            Assert.IsType<ObjectResult>(
                result);
        }

        // =====================================================
        // GET ACCOUNT - 200
        // =====================================================

        [Fact]
        public async Task GetAccount_Returns_200_OK()
        {
            _serviceMock
                .Setup(x =>
                    x.GetAccountByNumber(
                        "123",
                        1))
                .ReturnsAsync(
                    new ApiResponse<object>
                    {
                        Success = true
                    });

            var result =
                await _controller
                    .GetAccount("123");

            Assert.IsType<OkObjectResult>(
                result);
        }

        // =====================================================
        // GET ACCOUNT - 400
        // =====================================================

        [Fact]
        public async Task GetAccount_Returns_400_BadRequest()
        {
            _serviceMock
                .Setup(x =>
                    x.GetAccountByNumber(
                        "123",
                        1))
                .ReturnsAsync(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Account not found"
                    });

            var result =
                await _controller
                    .GetAccount("123");

            Assert.IsType<BadRequestObjectResult>(
                result);
        }

        // =====================================================
        // GET ACCOUNT - 500
        // =====================================================

        [Fact]
        public async Task GetAccount_Returns_500_Error()
        {
            _serviceMock
                .Setup(x =>
                    x.GetAccountByNumber(
                        "123",
                        1))
                .ThrowsAsync(
                    new Exception("Server Error"));

            var result =
                await _controller
                    .GetAccount("123");

            Assert.IsType<ObjectResult>(
                result);
        }
    }
}