
namespace SurveyBasket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
//[Authorize(Roles = "Admin")]
public class ResultsController(IResultService resultService) : ControllerBase
{
    private readonly IResultService _resultService = resultService;

    [HttpGet("/api/polls/{pollId}/results")]
    public async Task<IActionResult> GetPollResults([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var result = await _resultService.GetPollResultsAsync(pollId, cancellationToken);

        if (result.IsFailure)
            return result.ToProblem();

        return Ok(result.Value);
    }

    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview(CancellationToken cancellationToken)
    {
        var result = await _resultService.GetOverviewAsync(cancellationToken);

        if (result.IsFailure)
            return result.ToProblem();

        return Ok(result.Value);
    }

    [HttpGet("/api/results/votes-per-day")]
    public async Task<IActionResult> GetVotesPerDay(CancellationToken cancellationToken)
    {
        var result = await _resultService.GetVotesPerDayAsync(cancellationToken);

        if (result.IsFailure)
            return result.ToProblem();

        return Ok(result.Value);
    }

    [HttpGet("/api/results/top-polls")]
    public async Task<IActionResult> GetTopPolls(CancellationToken cancellationToken)
    {
        var result = await _resultService.GetTopPollsAsync(cancellationToken);

        if (result.IsFailure)
            return result.ToProblem();

        return Ok(result.Value);
    }

}