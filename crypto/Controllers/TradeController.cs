using crypto.Interfaces;
using crypto.Models;
using crypto.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace crypto.Controllers
{
    [ApiController]
    [Route("api/trade")]
    public class TradeController : ControllerBase
    {
        private readonly ITradeService _tradeService;

        public TradeController(ITradeService tradeService)
        {
            _tradeService = tradeService;
        }

        [HttpPost("buy")]
        public async Task<ActionResult<TransactionDto>> BuyCrypto([FromBody] TradeRequestDto request) 
        {
            // Add basic validation
            if (request.UserId <= 0 || request.CryptoId <= 0 || request.Amount <= 0)
            {
                return BadRequest("Invalid trade request data.");
            }

            try
            {
                var transaction = await _tradeService.BuyCryptoAsync(request.UserId, request.CryptoId, request.Amount);
                if (transaction == null)
                {
                    return NotFound("User or cryptocurrency not found, or other issue occurred.");
                }
                return Ok(transaction); 
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("sell")]
        public async Task<ActionResult<TransactionDto>> SellCrypto([FromBody] TradeRequestDto request) 
        {
            if (request.UserId <= 0 || request.CryptoId <= 0 || request.Amount <= 0)
            {
                return BadRequest("Invalid trade request data.");
            }

            try
            {
                var transaction = await _tradeService.SellCryptoAsync(request.UserId, request.CryptoId, request.Amount);
                if (transaction == null)
                {
                    return NotFound("User or cryptocurrency not found, or other issue occurred.");
                }
                return Ok(transaction); 
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}