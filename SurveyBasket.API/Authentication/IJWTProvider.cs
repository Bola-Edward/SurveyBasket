namespace SurveyBasket.API.Authentication;

public interface IJwtProvider
{
    (string token, int expiresIn) GenerateToken(ApplicationUser applicationUser);
    public string? ValidateToken(string token);
    public string? GetUserIdFromExpiredToken(string token);
}
