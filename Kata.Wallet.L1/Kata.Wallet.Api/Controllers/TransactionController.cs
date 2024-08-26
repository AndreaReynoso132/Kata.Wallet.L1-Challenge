using Kata.Wallet.Services;
using Kata.Wallet.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Kata.Wallet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Retrieves a list of transactions for a specific wallet.
        /// </summary>
        /// <param name="walletId">The ID of the wallet to retrieve transactions for.</param>
        /// <returns>Returns a list of transactions associated with the specified wallet.</returns>
        [HttpGet("wallet/{walletId}")]
        public async Task<IActionResult> GetTransactionsByWallet(int walletId)
        {
            var transactions = await _transactionService.GetTransactionsByWalletIdAsync(walletId);

            var transactionDtos = transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Date = t.Date,
                Description = t.Description
            }).ToList();

            return Ok(transactionDtos);
        }
    }
}
