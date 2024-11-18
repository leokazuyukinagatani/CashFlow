using CashFlow.Domain.Entities;

namespace WebApi.Tests.Resources;
public class ExpenseIdentityManager
{
    private readonly Expense _expense;

    public ExpenseIdentityManager(Expense expense)
    {
        _expense = expense;
    }

    public long GetId() => _expense.Id;
}
