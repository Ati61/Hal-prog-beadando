using crypto.Interfaces;
using crypto.Models;
using Microsoft.AspNetCore.Mvc;

namespace crypto.Controllers
{
    [ApiController]
    [Route("api")]
    public class PortfolioController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public PortfolioController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet("portfolio/{userId}")]
        public async Task<ActionResult<IEnumerable<WalletCrypto>>> GetUserPortfolio(int userId)
        {
            var portfolio = await _walletService.GetUserCryptosAsync(userId);
            return Ok(portfolio);
        }
    }
}