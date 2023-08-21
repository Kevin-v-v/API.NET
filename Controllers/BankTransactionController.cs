

using System.Security.Claims;
using BankAPI.Services;
using BankAPI.Data.BankModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BankAPI.Data.DTOs;

namespace BankAPI.Controllers;

[Authorize(Policy = "Client")]
[ApiController]
[Route("api/[controller]")]
public class BankTransactionController : ControllerBase
{
    private readonly BankTransactionService _bankTransactionService;
    private readonly ClientService _clientService;
    private readonly AccountService _accountService;
    public BankTransactionController(BankTransactionService bankTransactionService, ClientService clientService, AccountService accountService)
    {
        _bankTransactionService = bankTransactionService;
        _clientService = clientService;
        _accountService = accountService;

    }

    [HttpGet("accounts")]
    public async Task<IActionResult> GetAccounts(){
        string userEmail = User.FindFirst(ClaimTypes.Email).Value;
        Client? client = await _clientService.GetByEmail(userEmail);
        if(client == null)
        {
            return StatusCode(500);
        }
        var accounts = await _accountService.GetAccountsByClient(client.Id);
        return Ok(accounts);
    }
    
    [HttpPost]
    public async Task<IActionResult> NewWithdrawal(BankTransactionDtoIn withdrawalDto){
        string userEmail = User.FindFirst(ClaimTypes.Email).Value;
        Client? client = await _clientService.GetByEmail(userEmail);
        if(client == null)
        {
            return StatusCode(500);
        }
        string result = await _bankTransactionService.Withdrawal(withdrawalDto, client);

        if(result != "Success"){
            return BadRequest(new { message = result});
        }else{
            return NoContent();
        }

    }


}