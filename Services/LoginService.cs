using BankAPI.Data;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services;

public class LoginService{
    private readonly BankContext _context;
    public LoginService(BankContext context)
    {
        _context = context;
    }

    public async Task<Administrator?> GetAdmin(AdminDto admin)
    {
        return await _context.Administrators.SingleOrDefaultAsync(a => a.Email == admin.Email && a.Pwd == admin.Pwd);
    }

    public async Task<Client?> GetClient(ClientDto client)
    {
        return await _context.Clients.SingleOrDefaultAsync(a => a.Email == client.Email && a.Pwd == client.Pwd);
    }
} 