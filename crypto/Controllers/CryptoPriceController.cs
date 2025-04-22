using crypto.Interfaces;
using crypto.Models;
using Microsoft.AspNetCore.Mvc;

namespace crypto.Controllers
{
    [ApiController]
    [Route("api/crypto")]
    public class CryptoPriceController : ControllerBase
    {
        private readonly ICryptocurrencyService _cryptoService;

        public CryptoPriceController(ICryptocurrencyService cryptoService)
        {
            _cryptoService = cryptoService;
        }

        [HttpPut("price")]
        public async Task<ActionResult<Cryptocurrency>> UpdateCryptoPrice([FromBody] PriceUpdateRequest request)
        {
            var crypto = await _cryptoService.UpdateCryptoPriceAsync(request.CryptoId, request.NewPrice);
            if (crypto == null)
            {
                return NotFound();
            }
            return Ok(crypto);
        }

        [HttpGet("price/history/{cryptoId}")]
        public async Task<ActionResult<IEnumerable<PriceHistory>>> GetPriceHistory(int cryptoId)
        {
            var history = await _cryptoService.GetPriceHistoryAsync(cryptoId);
            return Ok(history);
        }
    }

    public class PriceUpdateRequest
    {
        public int CryptoId { get; set; }
        public decimal NewPrice { get; set; }
    }
}