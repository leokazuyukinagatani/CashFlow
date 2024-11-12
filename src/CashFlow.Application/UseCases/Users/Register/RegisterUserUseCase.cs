using AutoMapper;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Users;
using CashFlow.Domain.Security.Cryptography;
using CashFlow.Domain.Security.Tokens;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;
using FluentValidation.Results;

namespace CashFlow.Application.UseCases.Users.Register;

public class RegisterUserUseCase : IRegisterUserUseCase
{
    private readonly IMapper _mapper;
    private readonly IPassWordEncripter _passWordEncripter;
    private readonly IUsersReadOnlyRepository _usersReadOnlyRepository;
    private readonly IUsersWriteOnlyRepository _usersWriteOnlyRepository;
    private readonly IAccessTokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserUseCase(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IPassWordEncripter passWordEncripter, 
        IAccessTokenGenerator tokenGenerator,
        IUsersReadOnlyRepository usersReadOnlyRepository,
        IUsersWriteOnlyRepository usersWriteOnlyRepository)
    {
        _mapper = mapper;
        _passWordEncripter = passWordEncripter;
        _usersReadOnlyRepository = usersReadOnlyRepository;
        _tokenGenerator = tokenGenerator;
        _usersWriteOnlyRepository = usersWriteOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson request)
    {
        await ValidateAsync(request);

        var user = _mapper.Map<Domain.Entities.User>(request);
        user.Password = _passWordEncripter.Encrypt(user.Password.ToLower());
        user.UserIdentifier = Guid.NewGuid();

        await _usersWriteOnlyRepository.Add(user);

        await _unitOfWork.Commit();

        return new ResponseRegisteredUserJson
        {
            Name = user.Name,
            Token = _tokenGenerator.Generate(user)
        };
    }

    private async Task ValidateAsync(RequestRegisterUserJson request)
    {
        var result = new RegisterUserValidator().Validate(request);

        var emailExist = await _usersReadOnlyRepository.ExistActiveUserWithEmails(request.Email);

        if(emailExist)
        {
            result.Errors.Add(new ValidationFailure(string.Empty, ResourceErrorMessages.EMAIL_ALREADY_REGISTERED));
        }

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(f => f.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }


}
