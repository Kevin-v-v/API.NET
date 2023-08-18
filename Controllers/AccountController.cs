using BankAPI.Services;
using BankAPI.Data.BankModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using BankAPI.Data.DTOs;

namespace BankAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase {
    public readonly AccountService _accountService;
    public readonly AccountTypeService _accountTypeService;
    public readonly ClientService _clientService;

    public AccountController(AccountService accountService, AccountTypeService accountTypeService, ClientService clientService) 
    {
        _accountService = accountService;
        _accountTypeService = accountTypeService;
        _clientService = clientService;
    }

    [HttpGet]
    public async Task<IEnumerable<AccountDtoOut>> Get()
    {
        return await _accountService.GetAll();
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<AccountDtoOut>> GetById(int id){
        var account = await _accountService.GetDtoById(id);
        if (account == null){
            return AccountNotFound(id);
        }else{
            return Ok(account);
        }
        
    }
    [HttpPost]
    public async Task<IActionResult> Create(AccountDtoIn account){
        string validationResult = await validateAccount(account);
        if(validationResult != "Valid"){
            return BadRequest(new {message = validationResult});
        }
        Account newAccount = await _accountService.Create(account);
        return CreatedAtAction(nameof(GetById), new {id = newAccount.Id}, newAccount);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, AccountDtoIn account){
        if(id != account.Id)
            return BadRequest(new {message = $"El ID({id}) de la URL no coincide con el ID({account.Id}) del cuerpo de la solicitud."});
        

        Account? accountToUpdate = await _accountService.GetById(account.Id);
        if (accountToUpdate != null)
        {
            string validationResult = await validateAccount(account);
            if(validationResult != "Valid")
            {
                return BadRequest(new {message = validationResult});
            }
            await _accountService.Update(account);  
            return NoContent();
        }
        else
        {
            return AccountNotFound(account.Id);
        }
        

        
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id){
        
        var accountToDelete = await _accountService.GetById(id);
        if (accountToDelete != null){
            await _accountService.Delete(accountToDelete.Id);
            return Ok();
        }
        else{
            return AccountNotFound(id);
        }
    }

    public async Task<string> validateAccount(AccountDtoIn account){
        string result = "Valid";
        
        AccountType? accountType = await _accountTypeService.GetById(account.AccountType);
        if (accountType == null){
            return $"El tipo de cuenta {account.AccountType} no existe.";
        }

        int clientId = account.ClientId.GetValueOrDefault();
        Client? client = await _clientService.GetById(clientId);
        if (client == null){
            return $"El cliente {clientId} no existe.";
        }

        return result;
    }
    public NotFoundObjectResult AccountNotFound(int id)
    {
        return NotFound( new { message = $"La cuenta con ID = {id} no existe." });
    }
}