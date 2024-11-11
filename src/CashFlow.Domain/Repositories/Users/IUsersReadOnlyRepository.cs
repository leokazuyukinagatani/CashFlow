namespace CashFlow.Domain.Repositories.Users;
public interface IUsersReadOnlyRepository
{
    Task<bool> ExistActiveUserWithEmails(string email);

    Task<Entities.User?> GetUserByEmail(string email);
}
