using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zorent.BLL.DTOs.Account;
using Zorent.BLL.Interfaces;
using Zorent.Domain.Entities;

namespace Zorent.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _service;

        public AccountsController(IAccountService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateAccountDto dto)
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);
            return Ok(await _service.CreateAccount(dto, userId));
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> Get()
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);
            return Ok(await _service.GetUserAccounts(userId));
        }
    }
}
