
namespace SurveyBasket.API.Services.Implementations;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

    private readonly int _refreshTokenExpiry = 14;


    public async Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken)
    {
        // check the user is valid or not by the email
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return null;
        }
        // if the user is valid check the password is correct or not
        var IsValidPassword = await _userManager.CheckPasswordAsync(user, password);
        if (!IsValidPassword)
        {
            return null;
        }
        // if the email and password correct Generate the JWT Token

        var (token, expiresIn) = _jwtProvider.GenerateToken(user);


        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiry);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);

        // return the AuthResponse with the Informations
        return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn, refreshToken, refreshTokenExpiration);
    }

    public async Task<AuthResponse?> GetRefreshTokensAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.GetUserIdFromExpiredToken(token);

        if (userId is null)
        {
            return null;
        }

        var user = await _userManager.Users
            .Include(u => u.RefreshTokens)
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            return null;
        }

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
        {
            return null;
        }

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (newToken, expiresIn) = _jwtProvider.GenerateToken(user);


        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiry);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);

        return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newToken, expiresIn, newRefreshToken, refreshTokenExpiration);

    }

    public async Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
        {
            return false;
        }

        var user = await _userManager.Users
            .Include(u => u.RefreshTokens)
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            return false;
        }

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
        {
            return false;
        }

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded;

    }

    private static string GenerateRefreshToken()
    {
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        return refreshToken;
    }

}
