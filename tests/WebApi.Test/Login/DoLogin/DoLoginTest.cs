using CashFlow.Communication.Requests;
using System.Net;
using System.Text.Json;
using FluentAssertions;
using WebApi.Tests.InlineData;
using CommonTestUtilities.Requests;
using CashFlow.Exception;
using System.Globalization;

namespace WebApi.Tests.Login.DoLogin;

public class DoLoginTest : CashFlowClassFixture
{
    private const string METHOD = "api/login";

    private readonly string _email;
    private readonly string _name;
    private readonly string _password;
    

    public DoLoginTest(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    { 
        _email = webApplicationFactory.GetEmail();
        _name = webApplicationFactory.GetName();
        _password = webApplicationFactory.GetPassword();
    }

    [Fact]
    public async Task Success()
    {
        var request = new RequestLoginJson
        {
            Email = _email,
            Password = _password
        };

        var response = await DoPost(METHOD, request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var reponseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(reponseBody);

        responseData.RootElement.GetProperty("name").GetString().Should().Be(_name);
        responseData.RootElement.GetProperty("token").GetString().Should().NotBeNullOrWhiteSpace();

    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Login_Invalid(string culture)
    {
        var request = RequestLoginJsonBuilder.Build();

        var response = await DoPost(requestUri: METHOD,request: request,culture: culture);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var reponseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(reponseBody);

        var errors = responseData.RootElement.GetProperty("errorMessages").EnumerateArray();

        var expectedMessage = ResourceErrorMessages.ResourceManager.GetString("EMAIL_OR_PASSWORD_INVALID", new CultureInfo(culture));
        
        errors.Should().HaveCount(1).And.Contain(c => c.GetString()!.Equals(expectedMessage));

    }
}
