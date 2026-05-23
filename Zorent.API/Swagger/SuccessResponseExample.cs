using Swashbuckle.AspNetCore.Filters;
using Zorent.BLL.DTOs.Auth;

namespace Zorent.API.Swagger
{
    public class SuccessResponseExample
        : IExamplesProvider<SuccessResponseDto>
    {
        public SuccessResponseDto GetExamples()
        {
            return new SuccessResponseDto
            {
                Success = true,
                Message = "Success message",
                Data = "sample data"
            };
        }
    }
}