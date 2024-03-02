namespace OnlineStore.AuthService.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {

        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseWebSockets();
        app.MapGraphQL("/api/auth-service/graphql");

        app.Run();
    }
}
