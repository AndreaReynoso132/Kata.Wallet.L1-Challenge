using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Kata.Wallet.Services;
using Kata.Wallet.Database;
using Kata.Wallet.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Kata.Wallet.Tests
{
    [TestClass]
    public class TransactionServiceTests
    {
        private Mock<ITransactionRepository> _mockTransactionRepository;
        private ITransactionService _transactionService;

        [TestInitialize]
        public void Setup()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>();

            var transactions = new List<Transaction>
            {
                new Transaction
                {
                    Id = 1,
                    Amount = 100,
                    Date = DateTime.Now,
                    Description = "Transferencia a Wallet 2",
                    WalletIncoming = new Kata.Wallet.Domain.Wallet
                    {
                        Id = 2,
                        UserDocument = "123456789",
                        UserName = "Juan Perez",
                        Currency = Currency.USD 
                    },
                    WalletOutgoing = new Kata.Wallet.Domain.Wallet
                    {
                        Id = 1,
                        UserDocument = "987654321",
                        UserName = "Jose Lopez",
                        Currency = Currency.USD 
                    }
                },
                new Transaction
                {
                    Id = 2,
                    Amount = 50,
                    Date = DateTime.Now,
                    Description = "Transferencia desde Wallet 3",
                    WalletIncoming = new Kata.Wallet.Domain.Wallet
                    {
                        Id = 1,
                        UserDocument = "000000008",
                        UserName = "Rodrigo Lopez",
                        Currency = Currency.USD 
                    },
                    WalletOutgoing = new Kata.Wallet.Domain.Wallet
                    {
                        Id = 3,
                        UserDocument = "101010101",
                        UserName = "Federico Lopez",
                        Currency = Currency.USD 
                    }
                }
            };

            _mockTransactionRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(transactions);

            _transactionService = new TransactionService(_mockTransactionRepository.Object);
        }

        [TestMethod]
        public async Task GetTransactionsByWalletIdAsync_ReturnsCorrectTransactions()
        {
            var result = await _transactionService.GetTransactionsByWalletIdAsync(1);

           
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(t => t.WalletIncoming.Id == 1 || t.WalletOutgoing.Id == 1));
        }
    }
}
