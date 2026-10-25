
namespace SurveyBasket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var polls = await _pollService.GetAllAsync(cancellationToken);
        return Ok(polls);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Poll not found",
                detail: result.Error.Description);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] PollRequest request, CancellationToken cancellationToken = default)
    {
        var newPoll = await _pollService.AddAsync(request);

        return CreatedAtAction(nameof(GetById), new { id = newPoll.Id }, newPoll);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        var result = await _pollService.UpdateAsync(id, request, cancellationToken);

        if (result.IsFailure)
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Poll not found",
                detail: result.Error.Description);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.DeleteAsync(id, cancellationToken);
        if (result.IsFailure)
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Poll not found",
                detail: result.Error.Description);

        return NoContent();
    }

    [HttpPut("{id}/togglePublish")]
    public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
        if (result.IsFailure)
            if (result.IsFailure)
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Poll not found",
                    detail: result.Error.Description);

        return NoContent();
    }

}