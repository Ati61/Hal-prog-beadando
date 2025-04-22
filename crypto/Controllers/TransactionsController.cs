using crypto.Interfaces;
using crypto.Models;
using Microsoft.AspNetCore.Mvc;

namespace crypto.Controllers
{
    [ApiController]
    [Route("api")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITradeService _tradeService;

        public TransactionsController(ITradeService tradeService)
        {
            _tradeService = tradeService;
        }

        [HttpGet("transactions/{userId}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetUserTransactions(int userId)
        {
            var transactions = await _tradeService.GetUserTransactionsAsync(userId);
            return Ok(transactions);
        }

        [HttpGet("transactions/details/{transactionId}")]
        public async Task<ActionResult<Transaction>> GetTransactionDetails(int transactionId)
        {
            var transaction = await _tradeService.GetTransactionDetailsAsync(transactionId);
            if (transaction == null)
            {
                return NotFound();
            }
            return Ok(transaction);
        }

        [HttpGet("profit/{userId}")]
        public async Task<ActionResult> CalculateProfit(int userId)
        {
            var profit = await _tradeService.CalculateUserProfitAsync(userId);
            return Ok(profit);
        }

        [HttpGet("profit/details/{userId}")]
        public async Task<ActionResult> GetDetailedProfit(int userId)
        {
            var detailedProfit = await _tradeService.GetDetailedProfitAsync(userId);
            return Ok(detailedProfit);
        }
    }
}