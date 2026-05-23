using Xunit;

using Microsoft.AspNetCore.Mvc;

using Moq;

using Zorent.API.Controllers;

using Zorent.BLL.DTOs.Transaction;
using Zorent.BLL.Interfaces;

using Zorent.Common.Responses;

namespace Zorent.Tests.Controllers
{
    public class TransferControllerTests
    {
        private readonly Mock<ITransactionService>
            _serviceMock;

        private readonly TransferController
            _controller;

        public TransferControllerTests()
        {
            _serviceMock =
                new Mock<ITransactionService>();

            _controller =
                new TransferController(
                    _serviceMock.Object
                );
        }

        // =====================================================
        // TRANSFER - 200
        // =====================================================

        [Fact]
        public async Task
        Transfer_Returns_200_OK()
        {
            // ARRANGE

            var dto =
                new TransferDto
                {
                    Amount = 1000
                };

            var response =
                new ApiResponse
                {
                    Success = true,

                    Message =
                        "Transfer successful"
                };

            _serviceMock
                .Setup(x =>
                    x.Transfer(dto))
                .ReturnsAsync(
                    response);

            // ACT

            var result =
                await _controller
                    .Transfer(dto);

            // ASSERT

            var okResult =
                Assert.IsType<OkObjectResult>(
                    result);

            Assert.Equal(
                200,
                okResult.StatusCode);
        }

        // =====================================================
        // TRANSFER - 400
        // =====================================================

        [Fact]
        public async Task
        Transfer_Returns_400_BadRequest()
        {
            // ARRANGE

            var dto =
                new TransferDto
                {
                    Amount = 1000
                };

            var response =
                new ApiResponse
                {
                    Success = false,

                    Message =
                        "Transfer failed"
                };

            _serviceMock
                .Setup(x =>
                    x.Transfer(dto))
                .ReturnsAsync(
                    response);

            // ACT

            var result =
                await _controller
                    .Transfer(dto);

            // ASSERT

            var badRequest =
                Assert.IsType
                    <BadRequestObjectResult>(
                        result);

            Assert.Equal(
                400,
                badRequest.StatusCode);
        }

        // =====================================================
        // TRANSFER - 500
        // =====================================================

        [Fact]
        public async Task
        Transfer_Returns_500_Error()
        {
            // ARRANGE

            var dto =
                new TransferDto
                {
                    Amount = 1000
                };

            _serviceMock
                .Setup(x =>
                    x.Transfer(dto))
                .ThrowsAsync(
                    new Exception(
                        "Server error"
                    ));

            // ACT

            var result =
                await _controller
                    .Transfer(dto);

            // ASSERT

            var objectResult =
                Assert.IsType<ObjectResult>(
                    result);

            Assert.Equal(
                500,
                objectResult.StatusCode);
        }
    }
}