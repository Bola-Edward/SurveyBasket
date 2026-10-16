
namespace SurveyBasket.API.Authentication;

public class JwtProvider(IOptions<JwtOptions> jwtOptions) : IJwtProvider
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public (string token, int expiresIn) GenerateToken(ApplicationUser user)
    {
        Claim[] claims =
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

        var singingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);


        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresInMinutes),
            signingCredentials: singingCredentials
        );

        return (token: new JwtSecurityTokenHandler().WriteToken(token), expiresIn: _jwtOptions.ExpiresInMinutes * 60);
    }


    public string? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var symmetricSecurityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.Key));

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = symmetricSecurityKey,
                ValidateIssuerSigningKey = true,

                ValidIssuer = _jwtOptions.Issuer,
                ValidateIssuer = true,

                ValidAudience = _jwtOptions.Audience,
                ValidateAudience = true,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero

            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            return jwtToken.Claims
                .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
        }
        catch (SecurityTokenException)
        {
            return null;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }


    public string? GetUserIdFromExpiredToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var symmetricSecurityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.Key));

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = symmetricSecurityKey,
                ValidateIssuerSigningKey = true,

                ValidIssuer = _jwtOptions.Issuer,
                ValidateIssuer = true,

                ValidAudience = _jwtOptions.Audience,
                ValidateAudience = true,

                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            return jwtToken.Claims
                .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
        }
        catch (SecurityTokenException)
        {
            return null;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }


}
