

using System.Security.Claims;
using BankAPI.Data;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services;

public class BankTransactionService{
    private readonly BankContext _context;
    private readonly AccountService _accountService;
    public BankTransactionService(BankContext context, AccountService accountService)
    {
        _context = context;
        _accountService = accountService;
    }

    public async Task MakeTransaction(BankTransaction transaction, Account account){


        _context.BankTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        if(transaction.TransactionType == 2 || transaction.TransactionType == 4)
        {
            account.Balance -= transaction.Amount;
        }
        else
        {
            account.Balance += transaction.Amount;
        }
        await _context.SaveChangesAsync();
        return;
    }
    

    public async Task<string> Withdrawal(BankTransactionDtoIn withdrawalDto, Client client)
    {
        Account? account = await _accountService.GetById(withdrawalDto.AccountId);
        if(account == null){
            return "Cuenta no existe.";
        }
        if(account.ClientId != client.Id){
            return "Cuenta no pertenece a usuario.";
        }
        if(account.Balance < withdrawalDto.Amount){
            return "Cuenta no posee fondos suficientes";
        }
        int transactionType;
        if(withdrawalDto.ExternalAccount != null && withdrawalDto.ExternalAccount != 0)
        {
            transactionType = 4;
        }else{
            transactionType = 2;
        }

        BankTransaction transaction = new BankTransaction();
        transaction.AccountId = withdrawalDto.AccountId;
        transaction.Amount = withdrawalDto.Amount;
        transaction.ExternalAccount = transactionType == 4 ? withdrawalDto.ExternalAccount : null;
        transaction.TransactionType = transactionType;

        await MakeTransaction(transaction, account);

        return "Success";   
        
    }

    public async Task DeleteAccount(int accountId){
        Account? account = await _accountService.GetById(accountId);
        if(account != null){
            //para borrar la cuenta hay que borrar sus transacciones primero por la restriccion de llaves foraneas
            _context.BankTransactions.RemoveRange(_context.BankTransactions.Where(x => x.AccountId == accountId));
            await _context.SaveChangesAsync();
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
        }
        
    }
}