using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories.Users;
using CashFlow.Domain.Security.Cryptography;
using CashFlow.Domain.Security.Tokens;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Login;

public class DoLoginUseCase : IDoLoginUseCase
{
    private readonly IUsersReadOnlyRepository _repository;
    private readonly IPassWordEncripter _passWordEncripter;
    private readonly IAccessTokenGenerator _accessTokenGenerator;

    public DoLoginUseCase(
        IUsersReadOnlyRepository repository,
        IPassWordEncripter passWordEncripter,
        IAccessTokenGenerator accessTokenGenerator)
    {
        _repository = repository;
        _passWordEncripter = passWordEncripter;
        _accessTokenGenerator = accessTokenGenerator;
    }

    public async Task<ResponseRegisteredUserJson> Execute(RequestLoginJson request)
    {
        var user = await _repository.GetUserByEmail(request.Email);

        if (user is null) { 
            throw new InvalidLoginException();
        }

        var passwordMatch = _passWordEncripter.Verify(request.Password, user.Password);

        //if (passwordMatch == false)
        //{
        //    throw new InvalidLoginException();
        //}

        return new ResponseRegisteredUserJson
        {
            Name = user.Name,
            Token = _accessTokenGenerator.Generate(user)
        };
    }

}
