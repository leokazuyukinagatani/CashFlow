namespace CashFlow.Domain.Repositories.Users;
public interface IUsersWriteOnlyRepository
{
    Task Add(Entities.User user);
}
