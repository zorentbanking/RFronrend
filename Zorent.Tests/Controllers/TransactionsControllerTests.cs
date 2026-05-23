using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

using System.Security.Claims;

using Xunit;

using Zorent.API.Controllers;

using Zorent.BLL.DTOs.Transaction;

using Zorent.BLL.Interfaces;

using Zorent.Common.Responses;

namespace Zorent.Tests.Controllers
{
    public class TransactionsControllerTests
    {
        private readonly Mock<ITransactionService>
            _serviceMock;

        private readonly TransactionsController
            _controller;

        public TransactionsControllerTests()
        {
            _serviceMock =
                new Mock<ITransactionService>();

            _controller =
                new TransactionsController(
                    _serviceMock.Object);

            var claims =
                new List<Claim>
                {
                    new Claim(
                        "UserId",
                        "1")
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
        // GET TRANSACTIONS - 200
        // =====================================================

        [Fact]
        public async Task
        Get_Returns_200_OK()
        {
            var response =
                new ApiResponse<object>
                {
                    Success = true,

                    Message =
                        "Transactions fetched",

                    Data = new { }
                };

            _serviceMock
                .Setup(x =>
                    x.GetTransactions(
                        1,
                        1,
                        "date",
                        "desc"))
                .ReturnsAsync(
                    response);

            var result =
                await _controller.Get(
                    1,
                    1,
                    "date",
                    "desc");

            Assert.IsType<OkObjectResult>(
                result);
        }

        // =====================================================
        // GET TRANSACTIONS - 400
        // =====================================================

        [Fact]
        public async Task
        Get_Returns_400_BadRequest()
        {
            var response =
                new ApiResponse<object>
                {
                    Success = false,

                    Message =
                        "Invalid account"
                };

            _serviceMock
                .Setup(x =>
                    x.GetTransactions(
                        1,
                        1,
                        "date",
                        "desc"))
                .ReturnsAsync(
                    response);

            var result =
                await _controller.Get(
                    1,
                    1,
                    "date",
                    "desc");

            Assert.IsType
                <BadRequestObjectResult>(
                    result);
        }

        // =====================================================
        // GET TRANSACTIONS - 500
        // =====================================================

        [Fact]
        public async Task
        Get_Returns_500_Error()
        {
            _serviceMock
                .Setup(x =>
                    x.GetTransactions(
                        1,
                        1,
                        "date",
                        "desc"))
                .ThrowsAsync(
                    new Exception(
                        "Server Error"));

            var result =
                await _controller.Get(
                    1,
                    1,
                    "date",
                    "desc");

            var objectResult =
                Assert.IsType<ObjectResult>(
                    result);

            Assert.Equal(
                500,
                objectResult.StatusCode);
        }

        // =====================================================
        // SEARCH - 200
        // =====================================================

        [Fact]
        public async Task
        Search_Returns_200_OK()
        {
            var dto =
                new TransactionSearchDto();

            var response =
                new ApiResponse<object>
                {
                    Success = true,

                    Message =
                        "Transactions found",

                    Data = new { }
                };

            _serviceMock
                .Setup(x =>
                    x.Search(
                        dto,
                        1))
                .ReturnsAsync(
                    response);

            var result =
                await _controller.Search(
                    dto);

            Assert.IsType<OkObjectResult>(
                result);
        }

        // =====================================================
        // SEARCH - 400
        // =====================================================

        [Fact]
        public async Task
        Search_Returns_400_BadRequest()
        {
            var dto =
                new TransactionSearchDto();

            var response =
                new ApiResponse<object>
                {
                    Success = false,

                    Message =
                        "No data"
                };

            _serviceMock
                .Setup(x =>
                    x.Search(
                        dto,
                        1))
                .ReturnsAsync(
                    response);

            var result =
                await _controller.Search(
                    dto);

            Assert.IsType
                <BadRequestObjectResult>(
                    result);
        }

        // =====================================================
        // SEARCH - 500
        // =====================================================

        [Fact]
        public async Task
        Search_Returns_500_Error()
        {
            var dto =
                new TransactionSearchDto();

            _serviceMock
                .Setup(x =>
                    x.Search(
                        dto,
                        1))
                .ThrowsAsync(
                    new Exception(
                        "Server Error"));

            var result =
                await _controller.Search(
                    dto);

            var objectResult =
                Assert.IsType<ObjectResult>(
                    result);

            Assert.Equal(
                500,
                objectResult.StatusCode);
        }

        // =====================================================
        // EXPORT CSV - 200
        // =====================================================

        [Fact]
        public async Task
        Export_Returns_200_OK()
        {
            var dto =
                new TransactionSearchDto();

            byte[] file =
                System.Text.Encoding
                    .UTF8
                    .GetBytes(
                        "test");

            _serviceMock
                .Setup(x =>
                    x.ExportToCsv(
                        dto,
                        1))
                .ReturnsAsync(
                    file);

            var result =
                await _controller.Export(
                    dto);

            Assert.IsType<FileContentResult>(
                result);
        }

        // =====================================================
        // EXPORT CSV - 500
        // =====================================================

        [Fact]
        public async Task
        Export_Returns_500_Error()
        {
            var dto =
                new TransactionSearchDto();

            _serviceMock
                .Setup(x =>
                    x.ExportToCsv(
                        dto,
                        1))
                .ThrowsAsync(
                    new Exception(
                        "Server Error"));

            var result =
                await _controller.Export(
                    dto);

            var objectResult =
                Assert.IsType<ObjectResult>(
                    result);

            Assert.Equal(
                500,
                objectResult.StatusCode);
        }

        // =====================================================
        // GET STATEMENT - 200
        // =====================================================

        [Fact]
        public async Task
        GetStatement_Returns_200_OK()
        {
            var response =
                new ApiResponse<object>
                {
                    Success = true,

                    Message =
                        "Statement fetched",

                    Data = new { }
                };

            _serviceMock
                .Setup(x =>
                    x.GetStatement(
                        "ACC001"))
                .ReturnsAsync(
                    response);

            var result =
                await _controller
                    .GetStatement(
                        "ACC001");

            Assert.IsType<OkObjectResult>(
                result);
        }

        // =====================================================
        // GET STATEMENT - 400
        // =====================================================

        [Fact]
        public async Task
        GetStatement_Returns_400_BadRequest()
        {
            var response =
                new ApiResponse<object>
                {
                    Success = false,

                    Message =
                        "Account not found"
                };

            _serviceMock
                .Setup(x =>
                    x.GetStatement(
                        "ACC001"))
                .ReturnsAsync(
                    response);

            var result =
                await _controller
                    .GetStatement(
                        "ACC001");

            Assert.IsType
                <BadRequestObjectResult>(
                    result);
        }

        // =====================================================
        // GET STATEMENT - 500
        // =====================================================

        [Fact]
        public async Task
        GetStatement_Returns_500_Error()
        {
            _serviceMock
                .Setup(x =>
                    x.GetStatement(
                        "ACC001"))
                .ThrowsAsync(
                    new Exception(
                        "Server Error"));

            var result =
                await _controller
                    .GetStatement(
                        "ACC001");

            var objectResult =
                Assert.IsType<ObjectResult>(
                    result);

            Assert.Equal(
                500,
                objectResult.StatusCode);
        }
    }
}