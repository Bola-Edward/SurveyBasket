using Microsoft.AspNetCore.WebUtilities;
using SurveyBasket.API.Helpers;

namespace SurveyBasket.API.Services.Implementations;

public class AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AuthService> logger, IEmailService emailService, IConfiguration configuration, IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IEmailService _emailService = emailService;
    private readonly IConfiguration _configuration = configuration;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

    private readonly int _refreshTokenExpiry = 14;


    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken)
    {
        // check the user is valid or not by the email
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return Result<AuthResponse>.Failure(UserErrors.InvalidCredentials);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

        if (result.Succeeded)
        {
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
            var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn, refreshToken, refreshTokenExpiration);

            // return the AuthResponse with the Informations
            return Result<AuthResponse>.Success(response);
        }

        return Result<AuthResponse>.Failure(result.IsNotAllowed ? AuthErrors.EmailNotConfirmed : UserErrors.InvalidCredentials);
    }

    public async Task<Result<AuthResponse>> GetRefreshTokensAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.GetUserIdFromExpiredToken(token);

        if (userId is null)
        {
            return Result<AuthResponse>.Failure(AuthErrors.InvalidToken);
        }

        var user = await _userManager.Users
            .Include(u => u.RefreshTokens)
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            return Result<AuthResponse>.Failure(AuthErrors.InvalidToken);
        }

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
        {
            return Result<AuthResponse>.Failure(AuthErrors.InvalidRefreshToken);
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

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            return Result<AuthResponse>.Failure(
                AuthErrors.RefreshTokenUpdateFailed);
        }

        return Result<AuthResponse>.Success(new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newToken, expiresIn, newRefreshToken, refreshTokenExpiration));

    }

    public async Task<Result> RegisterAsync(Contracts.Authentication.Requests.RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var emailIsExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

        if (emailIsExist)
        {
            return Result.Failure(UserErrors.DuplicatedEmail);
        }

        var user = request.Adapt<ApplicationUser>();

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Confirmation Code: {code}", code);

            await SendConfirmationEmail(user, code);

            return Result.Success();
        }

        return Result.Failure(AuthErrors.InvalidUser);
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        if (user.EmailConfirmed)
        {
            return Result.Failure(UserErrors.EmailAlreadyConfirmed);
        }

        try
        {
            var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));

            var result = await _userManager.ConfirmEmailAsync(user, decodedCode);

            if (result.Succeeded)
            {
                return Result.Success();
            }

            return Result.Failure(UserErrors.InvalidEmailConfirmationCode);
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidEmailConfirmationCode);
        }
    }

    public async Task<Result> ResendConfirmationEmailAsync(Contracts.Authentication.Requests.ResendConfirmationEmailRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        if (user.EmailConfirmed)
        {
            return Result.Failure(UserErrors.EmailAlreadyConfirmed);
        }

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Confirmation Code: {code}", code);

        await SendConfirmationEmail(user, code);

        return Result.Success();
    }

    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
        {
            return Result.Failure(AuthErrors.InvalidRefreshToken);
        }

        var user = await _userManager.Users
            .Include(u => u.RefreshTokens)
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(AuthErrors.InvalidToken);
        }

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
        {
            return Result.Failure(AuthErrors.InvalidRefreshToken);
        }

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return Result.Failure(AuthErrors.RefreshTokenUpdateFailed);
        }

        return Result.Success();

    }

    private static string GenerateRefreshToken()
    {
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        return refreshToken;
    }

    private async Task SendConfirmationEmail(ApplicationUser user, string code)
    {
        var origin = _configuration["FrontendUrl"];

        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
            templateModel: new Dictionary<string, string>
            {
            { "{{name}}", user.FirstName },
            { "{{action_url}}", $"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}" }
            }
        );

        await _emailService.SendAsync(user.Email!, "Survey Basket: Email Confirmation", emailBody);
    }

}
