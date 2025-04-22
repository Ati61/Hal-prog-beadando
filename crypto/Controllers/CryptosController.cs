using crypto.Interfaces;
using crypto.Models;
using Microsoft.AspNetCore.Mvc;

namespace crypto.Controllers
{
    [ApiController]
    [Route("api/cryptos")]
    public class CryptosController : ControllerBase
    {
        private readonly ICryptocurrencyService _cryptoService;

        public CryptosController(ICryptocurrencyService cryptoService)
        {
            _cryptoService = cryptoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cryptocurrency>>> GetAllCryptocurrencies()
        {
            var cryptos = await _cryptoService.GetAllCryptocurrenciesAsync();
            return Ok(cryptos);
        }

        [HttpGet("{cryptoId}")]
        public async Task<ActionResult<Cryptocurrency>> GetCryptocurrencyById(int cryptoId)
        {
            var crypto = await _cryptoService.GetCryptocurrencyByIdAsync(cryptoId);
            if (crypto == null)
            {
                return NotFound();
            }
            return Ok(crypto);
        }

        [HttpPost]
        public async Task<ActionResult<Cryptocurrency>> AddCryptocurrency(Cryptocurrency cryptocurrency)
        {
            var addedCrypto = await _cryptoService.AddCryptocurrencyAsync(cryptocurrency);
            return CreatedAtAction(nameof(GetCryptocurrencyById), new { cryptoId = addedCrypto.Id }, addedCrypto);
        }

        [HttpDelete("{cryptoId}")]
        public async Task<ActionResult> DeleteCryptocurrency(int cryptoId)
        {
            var result = await _cryptoService.DeleteCryptocurrencyAsync(cryptoId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}