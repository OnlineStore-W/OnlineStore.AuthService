namespace OnlineStore.AuthService.Core.Handlers.Login;

public record LoginResponse(string tokenId, DateTime expiration);
