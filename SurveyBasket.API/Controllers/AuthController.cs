
namespace SurveyBasket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("token")]
    public async Task<IActionResult> GetToken([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

        if (result is null)
            return BadRequest("Invalid Email or Password");

        return Ok(result);
    }


    [Authorize]
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new { Message = "Authenticated successfully!" });
    }
}