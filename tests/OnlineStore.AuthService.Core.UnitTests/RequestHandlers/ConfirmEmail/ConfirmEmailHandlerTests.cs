using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using OnlineStore.AuthService.Core.Abstractions;
using OnlineStore.AuthService.Core.Handlers.ConfirmEmail;
using OnlineStore.AuthService.Core.UnitTests.Helpers;
using OnlineStore.AuthService.Models;
using OnlineStore.CommonComponent.EventModels.UserModels;
using OnlineStore.CommonComponent.Kafka.Models;
using OnlineStore.CommonComponent.Kafka.Services;

namespace OnlineStore.AuthService.Core.UnitTests.RequestHandlers.ConfirmEmail;

public class ConfirmEmailHandlerTests
{
    private ServiceProviderBuilder serviceProviderBuilder;
    private ConfirmEmailHandler confirmEmailHandler;
    private Mock<ProduserService> produserServiceMock;
    private Mock<IProduserServiceWrapper> produserServiceWrapperMock;

    public ConfirmEmailHandlerTests()
    {
        serviceProviderBuilder = new ServiceProviderBuilder();

        var configData = new KafkaConfiguration
        {
            Brokers = "test",
            ConsumerGroup = "test",
            Key = "test",
            SchemaRegistryUrl = "",
            Topic = ""
        };

        var kafkaConfigMock = new Mock<IOptions<KafkaConfiguration>>();
        kafkaConfigMock.Setup(x => x.Value).Returns(configData);

        var produser = new ProduserService(kafkaConfigMock.Object);

        produserServiceMock = new Mock<ProduserService>(kafkaConfigMock.Object);
        produserServiceWrapperMock = new Mock<IProduserServiceWrapper>();
        produserServiceWrapperMock
            .Setup(x => x.ProduceAsync(It.IsAny<EmailConfermedEvent>()))
            .ReturnsAsync(true);

        confirmEmailHandler = new ConfirmEmailHandler(serviceProviderBuilder.BuildServiceProvider(), produserServiceWrapperMock.Object);
    }

    [Fact]
    public async Task Handle_WhenEmailConfiermed_ShouldReturnConfirmEmailResponse()
    {
        var user = GetAuthUser();

        serviceProviderBuilder
            .UserManagerMock
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

        serviceProviderBuilder
            .UserManagerMock
                .Setup(x => x.ConfirmEmailAsync(user, It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

        var request = GetConfirmEmailRequest();

        var result = await confirmEmailHandler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<ConfirmEmailResponse>(result);
        Assert.Equal(true, result.isConfirmed);
    }

    [Fact]
    public async Task Handle_WhenUserNotExist_ShouldThrowException()
    {
        var result = await Assert.ThrowsAsync<Exception>(async () => await confirmEmailHandler.Handle(GetConfirmEmailRequest(), CancellationToken.None));
        Assert.Equal("User does not exists!", result.Message);
    }

    [Fact]
    public async Task Handle_WhenEmailNotConfiermed_ShouldReturnConfirmEmailResponse()
    {
        var user = GetAuthUser();

        serviceProviderBuilder
            .UserManagerMock
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

        serviceProviderBuilder
            .UserManagerMock
                .Setup(x => x.ConfirmEmailAsync(user, It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

        var request = GetConfirmEmailRequest();

        var result = await confirmEmailHandler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<ConfirmEmailResponse>(result);
        Assert.Equal(false, result.isConfirmed);
    }

    private ConfirmEmailRequest GetConfirmEmailRequest()
        => new ConfirmEmailRequest()
        {
            Email = "test@test",
            ConfirmationToken = "testToken"
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
