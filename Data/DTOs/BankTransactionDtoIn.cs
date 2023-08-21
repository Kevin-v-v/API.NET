

namespace BankAPI.Data.DTOs;

public class BankTransactionDtoIn{
    public int AccountId { get; set;}
    public Decimal Amount { get; set;}
    public int? ExternalAccount { get; set;}
}