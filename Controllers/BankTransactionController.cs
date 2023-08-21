

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
    
    [HttpPost("retiro")]
    public async Task<IActionResult> NewWithdrawal(BankTransactionDtoIn transactionDto){
        string userEmail = User.FindFirst(ClaimTypes.Email).Value;
        Client? client = await _clientService.GetByEmail(userEmail);
        if(client == null)
        {
            return StatusCode(500);
        }

        int transactionType;
        if(transactionDto.ExternalAccount != null && transactionDto.ExternalAccount != 0){
            transactionType = 4; 
           
        }else{
            transactionType = 2;
        }

        BankTransaction transaction = new BankTransaction();
        transaction.AccountId = transactionDto.AccountId;
        transaction.Amount = transactionDto.Amount;
        transaction.ExternalAccount = transactionType == 3 ? transactionDto.ExternalAccount : null;
        transaction.TransactionType = transactionType;

        Account? account = await _accountService.GetById(transaction.AccountId);

        string result = ValidateTransaction(transaction, client, account);

        
        if(result != "Success"){
            return BadRequest(new { message = result});
        }else{
            await _bankTransactionService.MakeTransaction(transaction, account);
            return NoContent();
        }

    }

    [HttpPost("deposito")]
    public async Task<IActionResult> NewDeposit(BankTransactionDtoIn transactionDto){
        string userEmail = User.FindFirst(ClaimTypes.Email).Value;
        Client? client = await _clientService.GetByEmail(userEmail);
        if(client == null)
        {
            return StatusCode(500);
        }
        int transactionType;
        if(transactionDto.ExternalAccount != null && transactionDto.ExternalAccount != 0){
            //transactionType = 3; se haría si se permitiera este tipo de deposito por transferencia
            return BadRequest(new { message = "No se permite hacer depósitos por transferencia"});
        }else{
            transactionType = 1;
        }

        BankTransaction transaction = new BankTransaction();
        transaction.AccountId = transactionDto.AccountId;
        transaction.Amount = transactionDto.Amount;
        transaction.ExternalAccount = transactionType == 3 ? transactionDto.ExternalAccount : null;
        transaction.TransactionType = transactionType;

        Account? account = await _accountService.GetById(transaction.AccountId);

        string result = ValidateTransaction(transaction, client, account);
        
        

        if(result != "Success"){
            return BadRequest(new { message = result});
        }else{
            await _bankTransactionService.MakeTransaction(transaction, account);
            return NoContent();
        }

    }

    [HttpDelete("eliminar/{id}")]
    public async Task<IActionResult> DeleteAccount(int id){
        string userEmail = User.FindFirst(ClaimTypes.Email).Value;
        Client? client = await _clientService.GetByEmail(userEmail);
        if(client == null)
        {
            return StatusCode(500);
        }
        Account? account = await _accountService.GetById(id);
        if(account == null){
            return BadRequest(new { message = "Cuenta no existe."});
        }
        if(account.ClientId != client.Id){
            return BadRequest(new { message = "Cuenta no pertenece a usuario."});
        }
        if(account.Balance != 0){
            return BadRequest(new { message = "Cuenta debe vaciarse antes de ser eliminada."});
        }
        
        await _bankTransactionService.DeleteAccount(id);
        
        return NoContent();
        
        

    }

    private string ValidateTransaction(BankTransaction transaction, Client client, Account? account){
        
        if(account == null){
            return "Cuenta no existe.";
        }
        if(account.ClientId != client.Id){
            return "Cuenta no pertenece a usuario.";
        }

        if(transaction.TransactionType != 1 && transaction.TransactionType != 3 && account.Balance < transaction.Amount){
            return "Cuenta no posee fondos suficientes";
        }

        return "Success";
    }

}