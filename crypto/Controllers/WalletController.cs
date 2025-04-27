using crypto.Interfaces;
using crypto.Models;
using crypto.Dtos; 
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic; 

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
        public async Task<ActionResult<WalletDto>> GetWallet(int userId) 
        {
            var wallet = await _walletService.GetWalletByUserIdAsync(userId);
            if (wallet == null)
            {
                return NotFound();
            }
            return Ok(wallet);
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult<WalletDto>> UpdateWalletBalance(int userId, [FromBody] UpdateBalanceDto updateDto)
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }
            if (updateDto.NewBalance < 0)
            {
                return BadRequest("Balance cannot be negative.");
            }

            var wallet = await _walletService.UpdateWalletBalanceAsync(userId, updateDto.NewBalance);
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