using Microsoft.AspNetCore.Identity;
using Moq;
using OnlineStore.AuthService.Core.Abstractions;
using OnlineStore.AuthService.Core.Constants;
using OnlineStore.AuthService.Core.Handlers.RegisterAdmin;
using OnlineStore.AuthService.Core.UnitTests.Helpers;
using OnlineStore.AuthService.Models;

namespace OnlineStore.AuthService.Core.UnitTests.RequestHandlers.RegisterAdmin;

public class RegisterAdminHandlerTests
{
    private Mock<IEmailService> emailServiceMock;
    private ServiceProviderBuilder serviceProviderBuilder;
    private RegisterAdminHandler registerAdminHandler;

    public RegisterAdminHandlerTests()
    {
        serviceProviderBuilder = new ServiceProviderBuilder();

        emailServiceMock = new Mock<IEmailService>();
        emailServiceMock
            .Setup(x => x.SendEmail(It.IsAny<AuthUser>(), It.IsAny<string>()))
            .Returns(true);

        registerAdminHandler = new RegisterAdminHandler(serviceProviderBuilder.BuildServiceProvider(), emailServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WhenRegisterAdmin_ShouldReturnRegisterAdminResponse()
    {
        serviceProviderBuilder
            .UserManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<AuthUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

        serviceProviderBuilder
            .UserManagerMock
            .Setup(x => x.GetUsersInRoleAsync(UserRoles.Admin))
            .ReturnsAsync(new List<AuthUser>());

        var request = new RegisterAdminRequest()
        {
            Username = "testName",
            Email = "test@test",
            Password = "testPsw",
            ConfirmedPassword = "testPsw"
        };

        var result = await registerAdminHandler.Handle(request, CancellationToken.None);
        Assert.NotNull(result);
        Assert.IsType<RegisterAdminResponse>(result);
    }
}
