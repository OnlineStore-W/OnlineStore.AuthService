using Microsoft.AspNetCore.Identity;
using Moq;
using OnlineStore.AuthService.Core.Handlers.ChangePassword;
using OnlineStore.AuthService.Core.UnitTests.Helpers;
using OnlineStore.AuthService.Models;

namespace OnlineStore.AuthService.Core.UnitTests.RequestHandlers.ChangePassword;

public class ChangePasswordHandlerTests
{
    private ServiceProviderBuilder serviceProviderBuilder;
    private ChangePasswordHandler changePasswordHandler;

    public ChangePasswordHandlerTests()
    {
        serviceProviderBuilder = new ServiceProviderBuilder();

        changePasswordHandler = new ChangePasswordHandler(serviceProviderBuilder.BuildServiceProvider());
    }

    [Fact]
    public async Task Handle_WhenChangePassword_ShouldReturnChangePasswordResponse()
    {
        var user = GetAuthUser();

        serviceProviderBuilder
            .UserManagerMock
                .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

        serviceProviderBuilder
            .UserManagerMock
                .Setup(x => x.ChangePasswordAsync(user, It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

        var request = GetChangePasswordRequest();

        var result = await changePasswordHandler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<ChangePasswordResponse>(result);
        Assert.Equal(true, result.result);
    }

    [Fact]
    public async Task Handle_WhenUserNotExist_ShouldThrowException()
    {
        var result = await Assert.ThrowsAsync<Exception>(async () => await changePasswordHandler.Handle(GetChangePasswordRequest(), CancellationToken.None));
        Assert.Equal("User does not exists!", result.Message);
    }


    [Fact]
    public async Task Handle_WhenPasswordNotChangeed_ShouldThrowException()
    {
        var user = GetAuthUser();

        serviceProviderBuilder
            .UserManagerMock
                .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

        serviceProviderBuilder
            .UserManagerMock
                .Setup(x => x.ChangePasswordAsync(user, It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

        var result = await Assert.ThrowsAsync<Exception>(async () => await changePasswordHandler.Handle(GetChangePasswordRequest(), CancellationToken.None));
        Assert.NotNull(result);
    }

    private ChangePasswordRequest GetChangePasswordRequest()
        => new ChangePasswordRequest()
        {
            Username = "testName",
            OldPassword = "OldPassword",
            NewPassword = "NewPasswordw",
            ConfirmedNewPassword = "NewPassword"
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
