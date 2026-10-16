
namespace SurveyBasket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("token")]
    public async Task<IActionResult> GetToken([FromBody] Contracts.Authentication.Requests.LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

        if (result is null)
            return BadRequest("Invalid Email or Password");

        return Ok(result);
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var authResponse = await _authService.GetRefreshTokensAsync(request.Token, request.RefreshToken, cancellationToken);

        if (authResponse is null)
            return BadRequest("Invalid Token");

        return Ok(authResponse);
    }

    [HttpPut("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var isRevoked = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        if (!isRevoked)
            return BadRequest("Operation Failed");

        return NoContent();
    }

    [Authorize]
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new { Message = "Authenticated successfully!" });
    }
}