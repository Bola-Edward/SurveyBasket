
namespace SurveyBasket.API.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<AuthResponse?> GetRefreshTokensAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);

}