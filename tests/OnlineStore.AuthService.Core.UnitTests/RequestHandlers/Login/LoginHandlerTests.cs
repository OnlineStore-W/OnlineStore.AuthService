using Microsoft.Extensions.Configuration;
using Moq;
using OnlineStore.AuthService.Core.Constants;
using OnlineStore.AuthService.Core.Handlers.Login;
using OnlineStore.AuthService.Core.UnitTests.Helpers;
using OnlineStore.AuthService.Models;

namespace OnlineStore.AuthService.Core.UnitTests.RequestHandlers.Login;

public class LoginHandlerTests
{
    private ServiceProviderBuilder serviceProviderBuilder;
    private LoginHandler loginHandler;

    public LoginHandlerTests()
    {
        serviceProviderBuilder = new ServiceProviderBuilder();

        var inMemorySettings = new Dictionary<string, string>
        {
            {"JWT:ValidIssuer", "test"},
            {"JWT:ValidAudience", "test"},
            {"JWT:Secret", "JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyr"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        loginHandler = new LoginHandler(configuration, serviceProviderBuilder.BuildServiceProvider());
    }

    [Fact]
    public async Task Handle_WhenLogin_ShouldReturnLoginResponse()
    {
        var request = GetLoginRequest();
        var user = GetAuthUser();

        serviceProviderBuilder
            .UserManagerMock
                .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

        serviceProviderBuilder
            .UserManagerMock
                .Setup(x => x.CheckPasswordAsync(user, request.Password))
                .ReturnsAsync(true);

        serviceProviderBuilder
            .UserManagerMock
                .Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string>() { UserRoles.Manager });

        var result = await loginHandler.Handle(request, CancellationToken.None);
        Assert.NotNull(result);
        Assert.IsType<LoginResponse>(result);
    }

    [Fact]
    public async Task Handle_WhenUsernameOrPasswordNotCorrect_ShouldThrowException()
    {
        var result = await Assert.ThrowsAsync<Exception>(async () => await loginHandler.Handle(GetLoginRequest(), CancellationToken.None));
        Assert.Equal("Username or password is not correct.", result.Message);
    }

    [Fact]
    public async Task Handle_WhenEmailNotConfirmed_ShouldThrowException()
    {
        var user = GetAuthUser();
        user.EmailConfirmed = false;

        serviceProviderBuilder
            .UserManagerMock
                .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

        var result = await Assert.ThrowsAsync<Exception>(async () => await loginHandler.Handle(GetLoginRequest(), CancellationToken.None));
        Assert.Equal("Email is not confirmed.", result.Message);
    }

    [Fact]
    public async Task Handle_WhenAccessBlocked_ShouldThrowException()
    {
        var user = GetAuthUser();
        user.AccessFailedCount = 5;

        serviceProviderBuilder
            .UserManagerMock
                .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

        var result = await Assert.ThrowsAsync<Exception>(async () => await loginHandler.Handle(GetLoginRequest(), CancellationToken.None));
        Assert.Equal("Account is blocked", result.Message);
    }

    [Fact]
    public async Task Handle_WhenPasswordExpired_ShouldThrowException()
    {
        var user = GetAuthUser();
        user.PasswordExpiration = DateTime.UtcNow;

        serviceProviderBuilder
            .UserManagerMock
                .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

        var result = await Assert.ThrowsAsync<Exception>(async () => await loginHandler.Handle(GetLoginRequest(), CancellationToken.None));
        Assert.Equal("Password expired.", result.Message);
    }

    [Fact]
    public async Task Handle_WhenPasswordIncorect_ShouldThrowException()
    {
        var user = GetAuthUser();

        serviceProviderBuilder
            .UserManagerMock
                .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

        serviceProviderBuilder
            .UserManagerMock
                .Setup(x => x.CheckPasswordAsync(user, It.IsAny<string>()))
                .ReturnsAsync(false);

        var result = await Assert.ThrowsAsync<Exception>(async () => await loginHandler.Handle(GetLoginRequest(), CancellationToken.None));
        Assert.Equal("Unauthorized", result.Message);
    }

    private LoginRequest GetLoginRequest()
        => new LoginRequest()
        {
            Username = "testName",
            Password = "testPsw"
        };

    private AuthUser GetAuthUser()
        => new AuthUser()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testName",
            EmailConfirmed = true,
            AccessFailedCount = 0,
            PasswordExpiration = DateTime.UtcNow.AddYears(10)
        };
}
