using crypto.Interfaces;
using crypto.Models;
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
        public async Task<ActionResult<Transaction>> BuyCrypto([FromBody] TradeRequest request)
        {
            try
            {
                var transaction = await _tradeService.BuyCryptoAsync(request.UserId, request.CryptoId, request.Amount);
                if (transaction == null)
                {
                    return NotFound("User or cryptocurrency not found");
                }
                return Ok(transaction);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("sell")]
        public async Task<ActionResult<Transaction>> SellCrypto([FromBody] TradeRequest request)
        {
            try
            {
                var transaction = await _tradeService.SellCryptoAsync(request.UserId, request.CryptoId, request.Amount);
                if (transaction == null)
                {
                    return NotFound("User or cryptocurrency not found");
                }
                return Ok(transaction);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class TradeRequest
    {
        public int UserId { get; set; }
        public int CryptoId { get; set; }
        public decimal Amount { get; set; }
    }
}