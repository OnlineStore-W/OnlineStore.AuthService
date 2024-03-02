using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using OnlineStore.AuthService.Api.Extentions;
using OnlineStore.AuthService.DataAccess;

namespace OnlineStore.AuthService.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.SetGraphQL();

        builder.SetAuthentication();
        builder.SetMediatR();
        builder.Services.AddFluentValidationAutoValidation();

        builder.SetDBConfigs();
        builder.Services.RegisterConsumerServices();
        builder.AddKafkaService();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {

        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseWebSockets();
        app.MapGraphQL("/api/auth-service/graphql");

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();
            db.Database.Migrate();
        }

        app.Run();
    }
}
