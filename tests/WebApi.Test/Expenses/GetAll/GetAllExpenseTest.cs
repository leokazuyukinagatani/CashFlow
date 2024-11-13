using FluentAssertions;
using System.Text.Json;

namespace WebApi.Tests.Expenses.GetAll;
public class GetAllExpenseTest : CashFlowClassFixture
{
    private const string METHOD = "api/expenses";
    private readonly string _token;
    public GetAllExpenseTest(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
        _token = webApplicationFactory.User_Team_Member.GetToken();
    }

    [Fact]
    public async Task Success()
    {
        // Arrange
        // Act
        var result = await DoGet(requestUri:METHOD, token: _token);
        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        // Assert
        response.RootElement.GetProperty("expenses").EnumerateArray().Should().NotBeNullOrEmpty();

    }
}
