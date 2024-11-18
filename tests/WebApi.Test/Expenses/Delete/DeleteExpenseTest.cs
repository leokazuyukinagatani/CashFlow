using CashFlow.Exception;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Tests.InlineData;
using FluentAssertions;

namespace WebApi.Tests.Expenses.Delete;
public class DeleteExpenseTest : CashFlowClassFixture
{
    private const string METHOD = "api/expenses";

    private readonly string _token;
    private readonly long _expenseId;

    public DeleteExpenseTest(CustomWebApplicationFactory webApplicationFactory): base(webApplicationFactory)
    {
        _token = webApplicationFactory.User_Team_Member.GetToken();
        _expenseId = webApplicationFactory.Expense_MemberTeam.GetId();
    }

    [Fact]
    public async Task Success()
    {
        var requestUri = $"{METHOD}/{_expenseId}";
        var result = await DoDelete(requestUri: requestUri, token: _token);

        result.StatusCode.Should().Be(HttpStatusCode.NoContent);

        result = await DoGet(requestUri: requestUri, token: _token);

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Expense_Not_Found(string culture)
    {
        var requestUri = $"{METHOD}/1000";
        var result = await DoDelete(requestUri: requestUri, token: _token, culture: culture);

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        var expectedMessage = ResourceErrorMessages.ResourceManager.GetString("EXPENSE_NOT_FOUND", new CultureInfo(culture));

        errors.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(expectedMessage)); 
     
    }
}
