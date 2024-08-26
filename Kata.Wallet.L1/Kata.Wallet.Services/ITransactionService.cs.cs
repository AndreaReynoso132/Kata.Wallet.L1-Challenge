using Kata.Wallet.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kata.Wallet.Services
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetTransactionsByWalletIdAsync(int walletId);
    }
}
