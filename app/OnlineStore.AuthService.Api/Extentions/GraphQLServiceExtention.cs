using OnlineStore.AuthService.Api.Errors;
using OnlineStore.AuthService.Api.Schemas.Mutations;
using OnlineStore.AuthService.Api.Schemas.Queries;

namespace OnlineStore.AuthService.Api.Extentions;

public static class GraphQLServiceExtention
{
    public static void SetGraphQL(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddGraphQLServer()
            .AddQueryType(q => q.Name("Query"))
                .AddTypeExtension<AuthQuery>()
            .AddMutationType(q => q.Name("Mutation"))
                .AddTypeExtension<AuthMutation>()
            .AddAuthorization()
            .AddInMemorySubscriptions()
            .AddErrorFilter<GraphQLErrorFilter>();
    }
}