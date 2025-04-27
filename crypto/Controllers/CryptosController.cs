using crypto.Interfaces;
using crypto.Models;
using crypto.Dtos; 
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic; 

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
        public async Task<ActionResult<IEnumerable<CryptocurrencyDto>>> GetAllCryptocurrencies()
        {
            var cryptos = await _cryptoService.GetAllCryptocurrenciesAsync();
            return Ok(cryptos);
        }

        [HttpGet("{cryptoId}")]
        public async Task<ActionResult<CryptocurrencyDto>> GetCryptocurrencyById(int cryptoId) 
        {
            var crypto = await _cryptoService.GetCryptocurrencyByIdAsync(cryptoId);
            if (crypto == null)
            {
                return NotFound();
            }
            return Ok(crypto);
        }

        [HttpPost]
        public async Task<ActionResult<CryptocurrencyDto>> AddCryptocurrency(CryptocurrencyCreateDto cryptocurrencyDto) 
        {
            try
            {
                var addedCrypto = await _cryptoService.AddCryptocurrencyAsync(cryptocurrencyDto);
                return CreatedAtAction(nameof(GetCryptocurrencyById), new { cryptoId = addedCrypto.Id }, addedCrypto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
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