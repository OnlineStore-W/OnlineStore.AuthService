namespace OnlineStore.AuthService.Core.Abstractions;

public interface IProduserServiceWrapper
{
    public Task<bool> ProduceAsync<T>(T message) where T : class;
}
