namespace CashFlow.Domain.Security.Cryptography;
public interface IPassWordEncripter
{
    string Encrypt(string password);
}
