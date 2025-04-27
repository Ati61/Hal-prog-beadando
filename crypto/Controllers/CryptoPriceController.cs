using crypto.Interfaces;
using crypto.Models;
using crypto.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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
        public async Task<ActionResult<CryptocurrencyDto>> UpdateCryptoPrice([FromBody] PriceUpdateDto request)
        {
            
            if (request.CryptoId <= 0 || request.NewPrice < 0) //nem lehet negativ 
            {
                return BadRequest("Invalid price update request.");
            }

            var crypto = await _cryptoService.UpdateCryptoPriceAsync(request.CryptoId, request.NewPrice);
            if (crypto == null)
            {
                return NotFound();
            }
            return Ok(crypto);
        }

        [HttpGet("price/history/{cryptoId}")]
        public async Task<ActionResult<IEnumerable<PriceHistoryDto>>> GetPriceHistory(int cryptoId)
        {
            if (cryptoId <= 0)
            {
                return BadRequest("Invalid cryptocurrency ID.");
            }
            var history = await _cryptoService.GetPriceHistoryAsync(cryptoId);
            return Ok(history);
        }
    }
}