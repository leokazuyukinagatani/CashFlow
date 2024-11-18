using CashFlow.Domain.Entities;
using CashFlow.Domain.Enums;
using CashFlow.Domain.Security.Cryptography;
using CashFlow.Domain.Security.Tokens;
using CashFlow.Infrastructure.DataAccess;
using CommonTestUtilities.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Tests.Resources;

namespace WebApi.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public ExpenseIdentityManager Expense_MemberTeam { get; private set; } = default!;
    public ExpenseIdentityManager Expense_Admin { get; private set; } = default!;

    public UserIdentityManager User_Team_Member { get; private set; } = default!;
    public UserIdentityManager User_Admin { get; private set; } = default!;
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test")
            .ConfigureServices(services =>
            {
                var provider = services.AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

                services.AddDbContext<CashFlowDBContext>(config =>
                {
                    config.UseInMemoryDatabase("InMemoryDbForTesting");
                    config.UseInternalServiceProvider(provider);
                });

                var scope = services.BuildServiceProvider().CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<CashFlowDBContext>();
                var passwordEncripter = scope.ServiceProvider.GetRequiredService<IPasswordEncripter>();
                var tokenGenerator = scope.ServiceProvider.GetRequiredService<IAccessTokenGenerator>();


                StartDatabase(dbContext, passwordEncripter, tokenGenerator);
            });
    }
    
    private void StartDatabase(
        CashFlowDBContext dbContext, 
        IPasswordEncripter passwordEncripter,
        IAccessTokenGenerator accessTokenGenerator)
    {
        var userMember = AddUserTeamMember(dbContext, passwordEncripter, accessTokenGenerator);
        var expenseMember = AddExpenses(dbContext, userMember, expenseId: 1);
        Expense_MemberTeam = expenseMember;

        var userAdmin = AddUserAdmin(dbContext, passwordEncripter, accessTokenGenerator);
        var expenseAdmin = AddExpenses(dbContext, userAdmin, expenseId: 2);
        Expense_Admin = expenseAdmin;

        dbContext.SaveChanges();
    }

    private User AddUserTeamMember(
        CashFlowDBContext dbContext, 
        IPasswordEncripter passwordEncripter,
        IAccessTokenGenerator accesTokenGenerator)
    {
        var user = UserBuilder.Build();
        user.Id = 1;
        var password = user.Password;

        user.Password = passwordEncripter.Encrypt(user.Password);
        dbContext.Users.Add(user);

        var token = accesTokenGenerator.Generate(user);

        User_Team_Member = new UserIdentityManager(user, password, token);

        return user;
    }

    private User AddUserAdmin(
       CashFlowDBContext dbContext,
       IPasswordEncripter passwordEncripter,
       IAccessTokenGenerator accesTokenGenerator)
    {
        var user = UserBuilder.Build(Roles.ADMIN);
        user.Id = 2;

        var password = user.Password;

        user.Password = passwordEncripter.Encrypt(user.Password);
        dbContext.Users.Add(user);

        var token = accesTokenGenerator.Generate(user);

        User_Admin = new UserIdentityManager(user, password, token);

        return user;
    }


    private ExpenseIdentityManager AddExpenses(CashFlowDBContext dbContext, User user, long expenseId)
    {
        var expense = ExpenseBuilder.Build(user);

        expense.Id = expenseId;

        dbContext.Expenses.Add(expense);
        return new ExpenseIdentityManager(expense);
    }

}
