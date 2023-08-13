using BankAPI.Services;
using BankAPI.Data.BankModels;
using Microsoft.AspNetCore.Mvc;

namespace BankAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientController : ControllerBase {

    public readonly ClientService _clientService;
    public ClientController(ClientService clientService) {
        _clientService = clientService;
    }

    [HttpGet]
    public async Task<IEnumerable<Client>> Get(){
        return await _clientService.GetAll();
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Client>> GetById(int id){
        var client = await _clientService.GetById(id);
        if (client == null){
            return ClientNotFound(id);
        }else{
            return Ok(client);
        }
        
    }
    [HttpPost]
    public async Task<IActionResult> Create(Client client){
        Client newClient = await _clientService.Create(client);
        return CreatedAtAction(nameof(GetById), new {id = newClient.Id}, newClient);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Client client){
        if(id != client.Id)
            return BadRequest(new {message = $"El ID({id}) de la URL no coincide con el ID({client.Id}) del cuerpo de la solicitud."});

        Client? clientToUpdate = await _clientService.GetById(client.Id);
        if (clientToUpdate != null){
            await _clientService.Update(client);  
            return NoContent();
        }else{
            return ClientNotFound(client.Id);
        }
        

        
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id){
        
        var clientToDelete = await _clientService.GetById(id);
        if (clientToDelete != null){
            await _clientService.Delete(clientToDelete.Id);
            return Ok();
        }
        else{
            return ClientNotFound(id);
        }
    }

    public NotFoundObjectResult ClientNotFound(int id)
    {
        return NotFound( new { message = $"El cliente con ID = {id} no existe." });
    }
}

