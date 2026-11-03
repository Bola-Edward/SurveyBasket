
using SurveyBasket.API.Contracts.Vote.Requests;

namespace SurveyBasket.API.Controllers;

[Route("api/polls/{pollId}/[controller]")]
[ApiController]
[Authorize]
public class VotesController(IQuestionService questionService, IVoteService voteService) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;
    private readonly IVoteService _voteService = voteService;

    [HttpGet("")]
    public async Task<IActionResult> Start([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await _questionService.GetAvailableAsync(pollId, cancellationToken);

        if (result.IsFailure)
            return result.ToProblem();

        return Ok(result.Value);
    }

    [HttpPost("")]
    public async Task<IActionResult> Vote([FromRoute] int pollId, [FromBody] VoteRequest request, CancellationToken cancellationToken)
    {
        var result = await _voteService.AddAsync(pollId, User.GetUserId()!, request, cancellationToken);

        if (result.IsFailure)
            return result.ToProblem();

        return Created();
    }
}
