using AutoMapper;
using Kata.Wallet.Database;
using Kata.Wallet.Dtos;
using Kata.Wallet.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Kata.Wallet.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public WalletService(
            IMapper mapper,
            IWalletRepository walletRepository,
            ITransactionRepository transactionRepository)
        {
            _mapper = mapper;
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<List<Domain.Wallet>> GetAllAsync(string? currency = null, string? userDocument = null)
        {
            var wallets = await _walletRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(currency))
            {
                wallets = wallets.Where(w => w.Currency.ToString() == currency).ToList(); 
            }

            if (!string.IsNullOrEmpty(userDocument))
            {
                wallets = wallets.Where(w => w.UserDocument == userDocument).ToList(); 
            }

            return wallets.ToList();
        }


        public async Task CreateAsync(WalletDto walletDto)
        {
            var wallet = _mapper.Map<Domain.Wallet>(walletDto);
            await ValidateWalletDoesNotExist(wallet.Id);

            await _walletRepository.AddAsync(wallet);
        }

        private async Task ValidateWalletDoesNotExist(int walletId)
        {
            var existingWallet = await _walletRepository.GetByIdAsync(walletId);
            if (existingWallet != null)
            {
                throw new InvalidOperationException($"A wallet with ID {walletId} already exists. Please use a different ID.");
            }
        }

        public async Task TransferAsync(int sourceWalletId, int destinationWalletId, decimal amount)
        {
            ValidateTransferParameters(sourceWalletId, destinationWalletId, amount);

            var sourceWallet = await _walletRepository.GetByIdAsync(sourceWalletId);
            var destinationWallet = await _walletRepository.GetByIdAsync(destinationWalletId);

            ValidateWalletsForTransfer(sourceWallet, destinationWallet, amount);

            sourceWallet.Balance -= amount;
            destinationWallet.Balance += amount;

            await _walletRepository.UpdateAsync(sourceWallet);
            await _walletRepository.UpdateAsync(destinationWallet);

            await RecordTransaction(sourceWallet, destinationWallet, amount);
        }

        private void ValidateTransferParameters(int sourceWalletId, int destinationWalletId, decimal amount)
        {
            if (sourceWalletId == destinationWalletId)
            {
                throw new ArgumentException("Source and destination wallet IDs cannot be the same.");
            }

            if (amount <= 0)
            {
                throw new ArgumentException("Transfer amount must be positive.");
            }
        }

        private void ValidateWalletsForTransfer(Domain.Wallet sourceWallet, Domain.Wallet destinationWallet, decimal amount)
        {
            if (sourceWallet == null || destinationWallet == null)
            {
                throw new ArgumentException("One of the specified wallets does not exist.");
            }

            if (sourceWallet.Currency != destinationWallet.Currency)
            {
                throw new InvalidOperationException("Wallets must have the same currency to perform a transfer.");
            }

            if (sourceWallet.Balance < amount)
            {
                throw new InvalidOperationException("Insufficient balance in the source wallet.");
            }
        }

        private async Task RecordTransaction(Domain.Wallet sourceWallet, Domain.Wallet destinationWallet, decimal amount)
        {
            var transaction = new Transaction
            {
                Amount = amount,
                Date = DateTime.UtcNow,
                WalletIncoming = destinationWallet,
                WalletOutgoing = sourceWallet
            };

            await _transactionRepository.AddAsync(transaction);
        }
    }
}
