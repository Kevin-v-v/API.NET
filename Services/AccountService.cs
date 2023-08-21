using BankAPI.Data;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services;

public class AccountService 
{ 
    private readonly BankContext _context;
    private readonly ClientService _clientService;

    public AccountService(BankContext context, ClientService clientService) 
    {
        _context = context;
        _clientService = clientService;
    }
    public async Task<IEnumerable<AccountDtoOut>> GetAll()
    {
        return await _context.Accounts.Select(a => new AccountDtoOut
        {
            Id = a.Id,
            AccountName = a.AccountTypeNavigation.Name,
            ClientName = a.Client != null ? a.Client.Name : "",
            Balance = a.Balance,
            RegDate = a.RegDate
        }).ToListAsync();
    }

    public async Task<AccountDtoOut?> GetDtoById(int id)
    {
        return await _context.Accounts.Where(a => a.Id == id).Select(a => new AccountDtoOut
        {
            Id = a.Id,
            AccountName = a.AccountTypeNavigation.Name,
            ClientName = a.Client != null ? a.Client.Name : "",
            Balance = a.Balance,
            RegDate = a.RegDate
        }).SingleOrDefaultAsync();
    }

    public async Task<Account?> GetById(int id)
    {
        return await _context.Accounts.FindAsync(id);
    }
    public async Task<List<AccountDtoOut>?> GetAccountsByClient(int userId)
    {
        Client? client = await _clientService.GetById(userId);
        if(client == null)
        {
            return null;
        }
        return await _context.Accounts.Where(acc => acc.ClientId == client.Id).Select(a => new AccountDtoOut
        {
            Id = a.Id,
            AccountName = a.AccountTypeNavigation.Name,
            ClientName = a.Client != null ? a.Client.Name : "",
            Balance = a.Balance,
            RegDate = a.RegDate
        }).ToListAsync();
    }
    public async Task<Account> Create(AccountDtoIn account)
    {
        Account newAccount = new Account();
        newAccount.AccountType = account.AccountType;
        newAccount.ClientId = account.ClientId;
        newAccount.Balance = account.Balance;

        _context.Accounts.Add(newAccount);
        await _context.SaveChangesAsync();

        return newAccount ;
    }

    public async Task Update(AccountDtoIn account){
        
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