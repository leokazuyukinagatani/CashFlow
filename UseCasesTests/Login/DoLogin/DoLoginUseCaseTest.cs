using CashFlow.Application.UseCases.Login;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;
using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Token;
using FluentAssertions;

namespace UseCases.Tests.Login.DoLogin;

public class DoLoginUseCaseTest
{

    [Fact]
    public async Task Success()
    {
        // Arrange
        var request = RequestLoginJsonBuilder.Build();
        var user = UserBuilder.Build();
        request.Email = user.Email;

        var useCase = CreateUseCase(user, request.Password);

        // Act
        var result = await useCase.Execute(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(user.Name);
        result.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Error_User_Not_Found()
    {
        // Arrange
        var request = RequestLoginJsonBuilder.Build();
        var user = UserBuilder.Build();

        var useCase = CreateUseCase(user, request.Password);

        // Act
        var act = async () => await useCase.Execute(request);

        // Assert
        var result = await act.Should().ThrowAsync<InvalidLoginException>();

        result.Where(ex => ex.GetErrors().Count == 1 && ex.GetErrors().Contains(ResourceErrorMessages.EMAIL_OR_PASSWORD_INVALID));
    }

    [Fact]
    public async Task Error_Password_Not_Match()
    {
        // Arrange
        var request = RequestLoginJsonBuilder.Build();
        var user = UserBuilder.Build();
        request.Email = user.Email;

        var useCase = CreateUseCase(user);

        // Act
        var act = async () => await useCase.Execute(request);

        // Assert 
        var result = await act.Should().ThrowAsync<InvalidLoginException>();


        result.Where(ex => ex.GetErrors().Count == 1 && ex.GetErrors().Contains(ResourceErrorMessages.EMAIL_OR_PASSWORD_INVALID));



    }

    private DoLoginUseCase CreateUseCase(CashFlow.Domain.Entities.User user, string? password = null)
    {
        var passwordEncripter = new PasswordEncrypterBuilder().Verify(password).Build();
        var tokenGenerator = JwtTokenGeneratorBuilder.Build();
        var readRepository = new UserReadOnlyRepositoryBuilder().GetUserByEmail(user).Build();

        return new DoLoginUseCase(readRepository, passwordEncripter, tokenGenerator);
    }



}
