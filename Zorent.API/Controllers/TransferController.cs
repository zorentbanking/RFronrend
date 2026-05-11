using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zorent.BLL.DTOs.Transaction;
using Zorent.BLL.Interfaces;

namespace Zorent.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/transfer")]
    public class TransferController : ControllerBase
    {
        private readonly ITransactionService _service;

        public TransferController(ITransactionService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Transfer(TransferDto dto)
        {
            var result = await _service.Transfer(dto);

            if (!result.Success)
                return BadRequest(result); 

            return Ok(result);
        }
    }
}
