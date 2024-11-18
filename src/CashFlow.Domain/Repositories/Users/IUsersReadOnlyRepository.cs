namespace CashFlow.Domain.Repositories.Users;
public interface IUsersReadOnlyRepository
{
    Task<bool> ExistActiveUserWithEmail(string email);

    Task<Entities.User?> GetUserByEmail(string email);
}
