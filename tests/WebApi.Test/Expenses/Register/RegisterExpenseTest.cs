using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;
using System.Globalization;
using System.Text.Json;
using WebApi.Tests.InlineData;

namespace WebApi.Tests.Expenses.Register;
public class RegisterExpenseTest : CashFlowClassFixture
{
    private const string METHOD = "api/expenses";
    private readonly string _token;

    public RegisterExpenseTest(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
        _token = webApplicationFactory.User_Team_Member.GetToken();
    }

    [Fact]
    public async Task Success()
    {
        // Arrange
        var request = RequestRegisterExpenseJsonBuilder.Build();

        // Act
        var result = await DoPost(requestUri: METHOD,token : _token, request: request);

        // Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        var reponseBody = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(reponseBody);

        response.RootElement.GetProperty("title").GetString().Should().Be(request.Title);
    }


    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Title_Empty(string culture)
    {
        // Arrange
        var request = RequestRegisterExpenseJsonBuilder.Build();
        request.Title = string.Empty;
        
        // Act
        var result = await DoPost(requestUri: METHOD, request: request, token: _token, culture: culture);

        // Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        var reponseBody = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(reponseBody);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();
        var expectedMessage = ResourceErrorMessages.ResourceManager.GetString("TITLE_REQUIRED", new CultureInfo(culture));
           
        errors.Should().HaveCount(1).And.Contain(c => c.GetString()!.Equals(expectedMessage));
    }

}
