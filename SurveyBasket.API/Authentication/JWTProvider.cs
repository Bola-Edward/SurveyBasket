
namespace SurveyBasket.API.Authentication;

public class JWTProvider : IJWTProvider
{
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

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ggEcWxr6SjhBSgO6zk7SagMjqy2bC2xR"));

        var singingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var expiresIn = 30;

        var token = new JwtSecurityToken(
            issuer: "SurveyBasketApp",
            audience: "SurveyBasketApp users",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresIn),
            signingCredentials: singingCredentials
        );

        return (token: new JwtSecurityTokenHandler().WriteToken(token), expiresIn: expiresIn);
    }
}
