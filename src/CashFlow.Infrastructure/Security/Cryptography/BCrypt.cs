using CashFlow.Domain.Security.Cryptography;
using BC = BCrypt.Net.BCrypt;

namespace CashFlow.Infrastructure.Security.Cryptography;

internal class BCrypt : IPassWordEncripter
{
    public string Encrypt(string password)
    {
        string passwordHash = BC.HashPassword(password);

        return passwordHash;
    }

    public bool Verify(string password, string passwordHash) {

        var password1 = "suaSenha";
        var passwordHash1 = BC.HashPassword(password1);

        // Teste direto sem o banco
        var result = BC.Verify(password1, passwordHash1);
        Console.WriteLine(result);  // Deve exibir 'True'


        return BC.Verify(password, passwordHash);
    }

}
