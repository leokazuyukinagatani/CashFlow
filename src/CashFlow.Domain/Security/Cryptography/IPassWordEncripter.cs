namespace CashFlow.Domain.Security.Cryptography;
public interface IPassWordEncripter
{
    string Encrypt(string password);
    bool Verify(string password, string passwordHash);

}
