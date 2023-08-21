using BankAPI.Data;
using BankAPI.Data.BankModels;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services;

public class ClientService{

    private readonly BankContext _context;

    public ClientService(BankContext context){
        _context = context;
    }

    public async Task<IEnumerable<Client>> GetAll(){
        return await _context.Clients.ToListAsync();
    }

    public async Task<Client?> GetById(int id){
        return await _context.Clients.FindAsync(id);
    }

    public async Task<Client?> GetByEmail(string email){
        Client? client = await _context.Clients.Where(client => client.Email == email).SingleOrDefaultAsync();
        return client;
    }
    public async Task<Client> Create(Client client){

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return client;
    }

    public async Task Update(Client client){

        var existingClient = await GetById(client.Id);
        if(existingClient != null){
            existingClient.Name = client.Name;
            existingClient.Email = client.Email;
            existingClient.PhoneNumber = client.PhoneNumber;

            await _context.SaveChangesAsync();
        }
        
    }
    public async Task Delete(int id){
        var clientToDelete = await GetById(id);
        if(clientToDelete != null){
            _context.Clients.Remove(clientToDelete);
            await _context.SaveChangesAsync();
        }
        
    }
}