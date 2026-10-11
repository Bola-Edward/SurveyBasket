
namespace SurveyBasket.API.Services.Implementations;

public class PollService(ApplicationDbContext context) : IPollService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.polls
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Poll?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.polls.FindAsync(id, cancellationToken);
    }

    public async Task<Poll> AddAsync(Poll poll, CancellationToken cancellationToken = default)
    {
        await _context.polls.AddAsync(poll, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return poll;
    }

    public async Task<bool> UpdateAsync(int id, Poll poll, CancellationToken cancellationToken = default)
    {
        var currentPoll = await GetByIdAsync(id, cancellationToken);

        if (currentPoll is null)
            return false;

        currentPoll.Title = poll.Title;
        currentPoll.Summary = poll.Summary;
        currentPoll.StartsAt = poll.StartsAt;
        currentPoll.EndsAt = poll.EndsAt;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await GetByIdAsync(id, cancellationToken);

        if (poll is null)
            return false;

        poll.IsDeleted = true;
        poll.DeletedOnUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await GetByIdAsync(id, cancellationToken);

        if (poll is null)
            return false;

        poll.IsPublished = !poll.IsPublished;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
