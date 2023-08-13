using BankAPI.Data;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services;

public class AccountService 
{ 
    private readonly BankContext _context;

    public AccountService(BankContext context) 
    {
        _context = context;
    }
    public async Task<IEnumerable<Account>> GetAll()
    {
        return await _context.Accounts.ToListAsync();
    }

    public async Task<Account?> GetById(int id)
    {
        return await _context.Accounts.FindAsync(id);
    }

    public async Task<Account> Create(AccountDTO account)
    {
        Account newAccount = new Account();
        newAccount.AccountType = account.AccountType;
        newAccount.ClientId = account.ClientId;
        newAccount.Balance = account.Balance;

        _context.Accounts.Add(newAccount);
        await _context.SaveChangesAsync();

        return newAccount ;
    }

    public async Task Update(AccountDTO account){
        
        var existingAccount = await GetById(account.Id);
        if(existingAccount != null){
            existingAccount.AccountType = account.AccountType;
            existingAccount.ClientId = account.ClientId;
            existingAccount.Balance = account.Balance;

            await _context.SaveChangesAsync();
        }
        
    }
    public async Task Delete(int id){
        var accountToDelete = await GetById(id);
        if(accountToDelete != null){
            _context.Accounts.Remove(accountToDelete);
            await _context.SaveChangesAsync();
        }
        
    }
}