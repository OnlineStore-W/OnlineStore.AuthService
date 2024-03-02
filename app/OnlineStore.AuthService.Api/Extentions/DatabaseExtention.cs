using Microsoft.EntityFrameworkCore;
using OnlineStore.AuthService.DataAccess;

namespace OnlineStore.AuthService.Api.Extentions;

public static class DatabaseExtention
{
    public static void SetDBConfigs(this WebApplicationBuilder builder)
    {
        var postgressConnString = Environment.GetEnvironmentVariable("ConnectionString");
        if (string.IsNullOrEmpty(postgressConnString))
        {
            postgressConnString = builder.Configuration.GetConnectionString("ConnectionString");
        }

        builder.Services.AddDbContext<AuthenticationDbContext>(options => options.UseNpgsql(postgressConnString));
    }
}