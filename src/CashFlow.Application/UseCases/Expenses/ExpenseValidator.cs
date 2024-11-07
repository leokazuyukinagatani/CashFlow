using CashFlow.Communication.Requests;
using CashFlow.Exception;
using FluentValidation;

namespace CashFlow.Application.UseCases.Expenses;

public class ExpenseValidator : AbstractValidator<RequestExpenseJson>
{
    public ExpenseValidator()
    {
        RuleFor(expense => expense.Title).NotEmpty().WithMessage(ResourceErrorMessages.TITLE_REQUIRED);
        RuleFor(request => request.Description)
            .MaximumLength(200)
            .WithMessage(ResourceErrorMessages.DESCRIPTION_MUST_SHORTER_THAN_200)
            .When(request => !string.IsNullOrEmpty(request.Description));
        RuleFor(request => request.Amount).GreaterThan(0).WithMessage(ResourceErrorMessages.AMOUNT_MUST_GREATER_THAN_ZERO);
        RuleFor(request => request.Date).LessThanOrEqualTo(DateTime.UtcNow).WithMessage(ResourceErrorMessages.EXPENSES_CANNOT_FUTURE_DATE);
        RuleFor(request => request.PaymentType).IsInEnum().WithMessage(ResourceErrorMessages.PAYMENT_TYPE_INVALID);
    }

}
