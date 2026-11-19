
using SurveyBasket.API.Abstractions;

namespace SurveyBasket.API.Services.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> GetRefreshTokensAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> RegisterAsync(Contracts.Authentication.Requests.RegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);
    Task<Result> ResendConfirmationEmailAsync(Contracts.Authentication.Requests.ResendConfirmationEmailRequest request, CancellationToken cancellationToken = default);
    Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);

}