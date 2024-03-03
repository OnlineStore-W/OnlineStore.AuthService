using Microsoft.AspNetCore.Identity;
using Moq;
using OnlineStore.AuthService.Core.Abstractions;
using OnlineStore.AuthService.Core.Constants;
using OnlineStore.AuthService.Core.Handlers.RegisterManager;
using OnlineStore.AuthService.Core.UnitTests.Helpers;
using OnlineStore.AuthService.Models;

namespace OnlineStore.AuthService.Core.UnitTests.RequestHandlers.RegisterManager;

public class RegisterManagerHandlerTests
{
    private Mock<IEmailService> emailServiceMock;
    private ServiceProviderBuilder serviceProviderBuilder;
    private RegisterManagerHandler registerAdminHandler;

    public RegisterManagerHandlerTests()
    {
        serviceProviderBuilder = new ServiceProviderBuilder();

        emailServiceMock = new Mock<IEmailService>();
        emailServiceMock
            .Setup(x => x.SendEmail(It.IsAny<AuthUser>(), It.IsAny<string>()))
            .Returns(true);

        registerAdminHandler = new RegisterManagerHandler(serviceProviderBuilder.BuildServiceProvider(), emailServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WhenRegisterManager_ShouldReturnRegisterManagerResponse()
    {
        serviceProviderBuilder
            .UserManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<AuthUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

        var request = GetRegisterAdminRequest();

        var result = await registerAdminHandler.Handle(request, CancellationToken.None);
        Assert.NotNull(result);
        Assert.IsType<RegisterManagerResponse>(result);
        Assert.Equal("New manager created succesfully.", result.result);
    }

    [Fact]
    public async Task Handle_WhenUserExist_ShouldThrowException()
    {
        serviceProviderBuilder
            .UserManagerMock
                .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new AuthUser());

        var result = await Assert.ThrowsAsync<Exception>(async () => await registerAdminHandler.Handle(GetRegisterAdminRequest(), CancellationToken.None));
        Assert.Equal("User already exists!", result.Message);
    }

    [Fact]
    public async Task Handle_WhenEmailExist_ShouldThrowException()
    {
        serviceProviderBuilder
            .UserManagerMock
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new AuthUser());

        var result = await Assert.ThrowsAsync<Exception>(async () => await registerAdminHandler.Handle(GetRegisterAdminRequest(), CancellationToken.None));
        Assert.Equal("User with this email is already registered!", result.Message);
    }

    [Fact]
    public async Task Handle_WhenUserNotCreated_ShouldThrowException()
    {
        serviceProviderBuilder
            .UserManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<AuthUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

        serviceProviderBuilder
            .UserManagerMock
                .Setup(x => x.GetUsersInRoleAsync(UserRoles.Admin))
                .ReturnsAsync(new List<AuthUser>());

        var request = GetRegisterAdminRequest();

        var result = await Assert.ThrowsAsync<Exception>(async () => await registerAdminHandler.Handle(GetRegisterAdminRequest(), CancellationToken.None));
        Assert.NotNull(result);
    }

    private RegisterManagerRequest GetRegisterAdminRequest()
        => new RegisterManagerRequest()
        {
            Username = "testName",
            Email = "test@test",
            Password = "testPsw",
            ConfirmedPassword = "testPsw"
        };
}
