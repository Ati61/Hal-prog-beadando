using crypto.Interfaces;
using crypto.Models;
using crypto.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetUserTransactions(int userId) // Return DTO list
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }
            var transactions = await _tradeService.GetUserTransactionsAsync(userId);
            return Ok(transactions);
        }

        [HttpGet("transactions/details/{transactionId}")]
        public async Task<ActionResult<TransactionDto>> GetTransactionDetails(int transactionId) // Return DTO
        {
             if (transactionId <= 0)
            {
                return BadRequest("Invalid transaction ID.");
            }
            var transaction = await _tradeService.GetTransactionDetailsAsync(transactionId);
            if (transaction == null)
            {
                return NotFound();
            }
            return Ok(transaction);
        }

        [HttpGet("profit/{userId}")]
        public async Task<ActionResult<TotalProfitDto>> CalculateProfit(int userId)
        {
             if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }
            var profit = await _tradeService.CalculateUserProfitAsync(userId);
            return Ok(profit);
        }

        [HttpGet("profit/details/{userId}")]
        public async Task<ActionResult<DetailedProfitDto>> GetDetailedProfit(int userId) 
        {
             if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }
            var detailedProfit = await _tradeService.GetDetailedProfitAsync(userId);
            return Ok(detailedProfit);
        }
    }
}