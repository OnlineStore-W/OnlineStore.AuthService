using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OnlineStore.AuthService.Models;

namespace OnlineStore.AuthService.Core.UnitTests.Helpers;

public class ServiceProviderBuilder
{
    private Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
    public Mock<UserManager<AuthUser>> UserManagerMock { get; } = new Mock<UserManager<AuthUser>>(Mock.Of<IUserStore<AuthUser>>(), null, null, null, null, null, null, null, null);
    public Mock<RoleManager<IdentityRole>> RoleManagerMock { get; } = new Mock<RoleManager<IdentityRole>>(
             new Mock<IRoleStore<IdentityRole>>().Object, null, null, null, null);

    public ServiceProviderBuilder()
    {
        var serviceScope = new Mock<IServiceScope>();
        serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);

        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        serviceScopeFactory
            .Setup(x => x.CreateScope())
            .Returns(serviceScope.Object);

        serviceProviderMock
            .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(serviceScopeFactory.Object);

        serviceProviderMock
           .Setup(x => x.GetService(typeof(UserManager<AuthUser>)))
           .Returns(UserManagerMock.Object);

        serviceProviderMock
            .Setup(x => x.GetService(typeof(RoleManager<IdentityRole>)))
            .Returns(RoleManagerMock.Object);
    }

    public IServiceProvider BuildServiceProvider() => serviceProviderMock.Object;
}
