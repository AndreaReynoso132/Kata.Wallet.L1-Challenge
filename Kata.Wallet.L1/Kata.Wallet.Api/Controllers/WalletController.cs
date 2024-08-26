using Kata.Wallet.Dtos;
using Kata.Wallet.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kata.Wallet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    /// <summary>
    /// Retrieves a list of wallets filtered by currency and/or user document.
    /// </summary>
    /// <param name="currency">The currency code to filter wallets (optional).</param>
    /// <param name="userDocument">The user document to filter wallets (optional).</param>
    /// <returns>Returns a list of wallets matching the provided criteria or an error if no criteria are provided.</returns>
    [HttpGet("filter")]
    public async Task<IActionResult> GetWallets([FromQuery] string? currency = null, [FromQuery] string? userDocument = null)
    {
        var wallets = await _walletService.GetAllAsync(currency, userDocument);

        if (wallets == null || wallets.Count == 0)
        {
            return NotFound("No wallets found matching the provided criteria.");
        }

        return Ok(wallets);
    }

    /// <summary>
    /// Creates a new wallet.
    /// </summary>
    /// <param name="walletDto">The wallet data to create a new wallet.</param>
    /// <returns>Returns a success message if the wallet is created successfully, or an error message if the creation fails.</returns>
    [HttpPost("create")]
    public async Task<IActionResult> CreateWallet([FromBody] WalletDto walletDto)
    {
        try
        {
            await _walletService.CreateAsync(walletDto);
            return Ok("Wallet created successfully.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Transfers funds from one wallet to another.
    /// </summary>
    /// <param name="transferDto">The transfer details, including source and destination wallet IDs and the amount to transfer.</param>
    /// <returns>Returns a success message if the transfer is completed successfully, or an error message if the transfer fails.</returns>
    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferDto transferDto)
    {
        if (transferDto == null)
        {
            return BadRequest("Invalid transfer data.");
        }

        try
        {
            await _walletService.TransferAsync(transferDto.SourceWalletId, transferDto.DestinationWalletId, transferDto.Amount);
            return Ok("Transfer completed successfully.");
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
