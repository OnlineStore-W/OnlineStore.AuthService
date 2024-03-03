using OnlineStore.AuthService.Core.Abstractions;
using OnlineStore.CommonComponent.Kafka.Services;

namespace OnlineStore.AuthService.Core.Services;

public class ProduserServiceWrapper: IProduserServiceWrapper
{
    private readonly ProduserService produserService;

    public ProduserServiceWrapper(ProduserService produserService)
    {
        this.produserService = produserService ?? throw new ArgumentNullException();
    }

    public async Task<bool> ProduceAsync<T>(T message) where T : class
    {
        await this.produserService.ProduceAsync(message);

        return true;
    }
}
