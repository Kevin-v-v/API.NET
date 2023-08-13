namespace BankAPI.Data.DTOs;

public class AccountDTO
{
    public int Id {get; set;}
    public int AccountType  {get; set;}
    public int? ClientId {get; set;}
    public decimal Balance {get; set;}
}