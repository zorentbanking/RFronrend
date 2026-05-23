using Swashbuckle.AspNetCore.Filters;
using Zorent.BLL.DTOs.Auth;

namespace Zorent.API.Swagger
{
    public class ErrorResponseExample
        : IExamplesProvider<ErrorResponseDto>
    {
        public ErrorResponseDto GetExamples()
        {
            return new ErrorResponseDto
            {
                Success = false,
                Message = "Error message"
            };
        }
    }
}