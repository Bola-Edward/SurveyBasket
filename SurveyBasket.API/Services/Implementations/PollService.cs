
namespace SurveyBasket.API.Services.Implementations;

public class PollService(ApplicationDbContext context) : IPollService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var polls = await _context.polls
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return polls.Adapt<IEnumerable<PollResponse>>();
    }

    public async Task<Result<PollResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.polls.FindAsync(id, cancellationToken);

        if (poll is null)
            return Result<PollResponse>.Failure(PollErrors.NotFound);

        var response = poll.Adapt<PollResponse>();

        return Result<PollResponse>.Success(response);
    }

    public async Task<Result<PollResponse>> AddAsync(PollRequest request, CancellationToken cancellationToken = default)
    {
        var isExistingTitle = await _context.polls.AnyAsync(x => x.Title == request.Title, cancellationToken);

        if (isExistingTitle)
            return Result<PollResponse>.Failure(PollErrors.DuplicateTitle);

        var poll = request.Adapt<Poll>();

        await _context.polls.AddAsync(poll, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var response = poll.Adapt<PollResponse>();
        return Result<PollResponse>.Success(response);
    }

    public async Task<Result> UpdateAsync(int id, PollRequest poll, CancellationToken cancellationToken = default)
    {
        var currentPoll = await _context.polls.FindAsync(id, cancellationToken);

        if (currentPoll is null)
            return Result.Failure(PollErrors.NotFound);

        var isExistingTitle = await _context.polls.AnyAsync(x => x.Title == poll.Title && x.Id != id, cancellationToken);

        if (isExistingTitle)
            return Result.Failure(PollErrors.DuplicateTitle);

        currentPoll.Title = poll.Title;
        currentPoll.Summary = poll.Summary;
        currentPoll.StartsAt = poll.StartsAt;
        currentPoll.EndsAt = poll.EndsAt;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.polls.FindAsync(id, cancellationToken);

        if (poll is null)
            return Result.Failure(PollErrors.NotFound);

        poll.IsDeleted = true;
        poll.DeletedOnUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.polls.FindAsync(id, cancellationToken);

        if (poll is null)
            return Result.Failure(PollErrors.NotFound);

        poll.IsPublished = !poll.IsPublished;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
