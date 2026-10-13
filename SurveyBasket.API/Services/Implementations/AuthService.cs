namespace SurveyBasket.API.Services.Implementations;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

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

        // return the AuthResponse with the Informations
        return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn);
    }
}
