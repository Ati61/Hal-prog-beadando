using crypto.Interfaces;
using crypto.Models;
using Microsoft.AspNetCore.Mvc;

namespace crypto.Controllers
{
    [ApiController]
    [Route("api/wallet")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<Wallet>> GetWallet(int userId)
        {
            var wallet = await _walletService.GetWalletByUserIdAsync(userId);
            if (wallet == null)
            {
                return NotFound();
            }
            return Ok(wallet);
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult<Wallet>> UpdateWalletBalance(int userId, [FromBody] decimal newBalance)
        {
            var wallet = await _walletService.UpdateWalletBalanceAsync(userId, newBalance);
            if (wallet == null)
            {
                return NotFound();
            }
            return Ok(wallet);
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteWallet(int userId)
        {
            var result = await _walletService.DeleteWalletAsync(userId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}