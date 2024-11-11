using AutoMapper;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Security.Cryptography;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Users.Register;

public class RegisterUserUseCase : IRegisterUserUseCase
{
    private readonly IMapper _mapper;
    private readonly IPassWordEncripter _passWordEncripter;

    public RegisterUserUseCase(IMapper mapper, IPassWordEncripter passWordEncripter)
    {
        _mapper = mapper;
        _passWordEncripter = passWordEncripter;
    }

    public async Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson request)
    {
        Validate(request);

        var user = _mapper.Map<Domain.Entities.User>(request);
        user.Password = _passWordEncripter.Encrypt(user.Password);

        return new ResponseRegisteredUserJson
        {
            Name = user.Name,
        };
    }

    private void Validate(RequestRegisterUserJson request)
    {
        var result = new RegisterUserValidator().Validate(request);

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(f => f.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }


}
