using Kata.Wallet.Database;
using Kata.Wallet.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kata.Wallet.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<List<Transaction>> GetTransactionsByWalletIdAsync(int walletId)
        {
            var transactions = await _transactionRepository.GetAllAsync();
            return transactions
                .Where(t => (t.WalletIncoming != null && t.WalletIncoming.Id == walletId) ||
                            (t.WalletOutgoing != null && t.WalletOutgoing.Id == walletId))
                .ToList();
        }
    }
}
