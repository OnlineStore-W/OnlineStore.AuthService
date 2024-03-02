using OnlineStore.CommonComponent.Kafka.Models;
using OnlineStore.CommonComponent.Kafka.Services;

namespace OnlineStore.AuthService.Api.Extentions;

public static class KafkaRegistrator
{
    public static void RegisterConsumerServices(this IServiceCollection service)
    {

    }

    public static void AddKafkaService(this WebApplicationBuilder builder)
    {
        var kafkaConfig = builder.Configuration.GetSection(nameof(KafkaConfiguration));
        builder.Services.Configure<KafkaConfiguration>(kafkaConfig);

        builder.Services.AddTransient<ProduserService>();
    }
}