using Kata.Wallet.Dtos;

namespace Kata.Wallet.Services;

public interface IWalletService
{
    Task<List<Domain.Wallet>> GetAllAsync(string? currency = null, string? userDocument = null);  
    Task CreateAsync(WalletDto walletDto);
    Task TransferAsync(int sourceWalletId, int destinationWalletId, decimal amount); 
}
