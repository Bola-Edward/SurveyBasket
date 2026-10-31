
namespace SurveyBasket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("")]
    public async Task<IActionResult> LoginAsync([FromBody] Contracts.Authentication.Requests.LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

        if (result.IsFailure)
            return result.ToProblem();

        return Ok(result.Value);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.GetRefreshTokensAsync(request.Token, request.RefreshToken, cancellationToken);

        if (result.IsFailure)
            return result.ToProblem();

        return Ok(result.Value);
    }

    [HttpPut("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        if (result.IsFailure)
            return result.ToProblem();

        return NoContent();
    }

}