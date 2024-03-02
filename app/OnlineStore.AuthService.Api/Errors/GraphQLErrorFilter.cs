namespace OnlineStore.AuthService.Api.Errors;

public class GraphQLErrorFilter : IErrorFilter
{
    public IError OnError(IError error)
    {
        if (error.Exception != null)
        {
            return error.WithMessage(error.Exception.Message);
        }
        else
        {
            return error;
        }
    }
}