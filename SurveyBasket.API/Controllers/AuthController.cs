
namespace SurveyBasket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ILogger<AuthController> _logger = logger;

    [HttpPost("")]
    public async Task<IActionResult> Login([FromBody] Contracts.Authentication.Requests.LoginRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Logging with email: {email} and password: {password}", request.Email, request.Password);

        var result = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

        if (result.IsFailure)
            return result.ToProblem();

        return Ok(result.Value);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.GetRefreshTokensAsync(request.Token, request.RefreshToken, cancellationToken);

        if (result.IsFailure)
            return result.ToProblem();

        return Ok(result.Value);
    }

    [HttpPut("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        if (result.IsFailure)
            return result.ToProblem();

        return NoContent();
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Contracts.Authentication.Requests.RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);

        if (result.IsFailure)
            return result.ToProblem();

        return Ok();
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
    {
        var result = await _authService.ConfirmEmailAsync(request);

        if (result.IsFailure)
            return result.ToProblem();

        return Ok();
    }

    [HttpPost("resend-confirmation-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendConfirmationEmail(Contracts.Authentication.Requests.ResendConfirmationEmailRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.ResendConfirmationEmailAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return result.ToProblem();
        }

        return NoContent();
    }

}