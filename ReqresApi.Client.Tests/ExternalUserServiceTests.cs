using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using TestAssignment.Model;
using TestAssignment.Service;

public class ExternalUserServiceTests
{
    private readonly Mock<HttpMessageHandler> _handlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<ILogger<ExternalUserService>> _loggerMock;
    private readonly ExternalUserService _service;

    public ExternalUserServiceTests()
    {
        _configMock = new Mock<IConfiguration>();
        _configMock
            .Setup(c => c["ReqresApi:BaseUrl"])
            .Returns("https://reqres.in/api");

        _loggerMock = new Mock<ILogger<ExternalUserService>>();

        _handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _httpClient = new HttpClient(_handlerMock.Object);
        _service = new ExternalUserService(
            _httpClient,
            _configMock.Object,
            _loggerMock.Object
        );
    }

    [Fact(DisplayName = "GetUserById: returns a single user when ID = 2")]
    public async Task GetUserById_ReturnsCorrectUser()
    {
        var singlePayload = new
        {
            data = new
            {
                id = 2,
                email = "janet.weaver@reqres.in",
                first_name = "Janet",
                last_name = "Weaver",
                avatar = "https://reqres.in/img/faces/2-image.jpg"
            }
        };

        _handlerMock
           .Protected()
           .Setup<Task<HttpResponseMessage>>(
               "SendAsync",
               ItExpr.Is<HttpRequestMessage>(req =>
                   req.Method == HttpMethod.Get &&
                   req.RequestUri == new Uri("https://reqres.in/api/users/2")
               ),
               ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent(JsonConvert.SerializeObject(singlePayload))
           })
           .Verifiable();

        var result = await _service.GetUserByIdAsync(2);

        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );

        Assert.NotNull(result);
        Assert.Equal(2, result.id);
        Assert.Equal("Janet", result.first_name);
        Assert.Equal("Weaver", result.last_name);
        Assert.Equal("janet.weaver@reqres.in", result.email);
        
    }

    [Fact(DisplayName = "GetAllUsers: returns a paged list of users")]
    public async Task GetAllUsers_ReturnsPagedList()
    {

        var listPayload = new User
        {
            page = 1,
            per_page = 2,
            total = 12,
            total_pages = 6,
            data = new List<UserData>
            {
                new UserData {
                    id = 1,
                    email = "george.bluth@reqres.in",
                    first_name = "George",
                    last_name  = "Bluth",
                    avatar     = "https://reqres.in/img/faces/1-image.jpg"
                },
                new UserData {
                    id = 2,
                    email = "janet.weaver@reqres.in",
                    first_name = "Janet",
                    last_name  = "Weaver",
                    avatar     = "https://reqres.in/img/faces/2-image.jpg"
                }
            },
            support = new Support
            {
                url = "https://contentcaddy.io?utm_source=reqres&utm_medium=json&utm_campaign=referral",
                text = "support URL MOC"
            }
        };

        _handlerMock
           .Protected()
           .Setup<Task<HttpResponseMessage>>(
               "SendAsync",
               ItExpr.Is<HttpRequestMessage>(req =>
                   req.Method == HttpMethod.Get &&
                   req.RequestUri == new Uri($"https://reqres.in/api/users?page={2}")
               ),
               ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent(JsonConvert.SerializeObject(listPayload))
           })
           .Verifiable();


        var result = await _service.GetAllUsersAsync();

        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );

        Assert.True(result.Count > 0);
    
        // Validate first item
        Assert.Equal(1, result[0].id);
        Assert.Equal("George", result[0].first_name);
        Assert.Equal("Bluth", result[0].last_name);

        // Validate second item
        Assert.Equal(2, result[1].id);
        Assert.Equal("Janet", result[1].first_name);
        Assert.Equal("Weaver", result[1].last_name);

    }
}
