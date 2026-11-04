using SurveyBasket.API.Contracts.Results.Responses;

namespace SurveyBasket.API.Services.Implementations;

public class ResultService(ApplicationDbContext context) : IResultService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<PollResultResponse>> GetPollResultsAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var poll = await _context.polls
            .Where(x => x.Id == pollId)
            .Select(x => new
            {
                x.Id,
                x.Title,
                x.Summary
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (poll is null)
            return Result<PollResultResponse>.Failure(PollErrors.NotFound);

        var totalVotes = await _context.Votes.CountAsync(x => x.PollId == pollId, cancellationToken);

        var questions = await _context.Questions
            .IgnoreQueryFilters()
            .Where(q => q.PollId == pollId)
            .Select(q => new QuestionResultResponse(q.Id, q.Content, _context.Answers
            .IgnoreQueryFilters()
            .Where(a => a.QuestionId == q.Id)
            .Select(a => new
            {
                Answer = a,
                Votes = _context.VoteAnswers.Count(va => va.AnswerId == a.Id && va.QuestionId == q.Id),

                QuestionVotes = _context.VoteAnswers
                    .Where(va => va.QuestionId == q.Id)
                    .Select(va => va.VoteId)
                    .Distinct()
                    .Count()
            })
            .Select(x => new AnswerResultResponse(
                x.Answer.Id,
                x.Answer.Content,
                x.Votes,
                x.QuestionVotes == 0 ? 0 : Math.Round((double)x.Votes / x.QuestionVotes * 100, 2)
            )).ToList()

    )).ToListAsync(cancellationToken);

        var response = new PollResultResponse(poll.Id, poll.Title, poll.Summary, totalVotes, questions);

        return Result<PollResultResponse>.Success(response);
    }


    public async Task<Result<OverviewResponse>> GetOverviewAsync(CancellationToken cancellationToken = default)
    {
        var totalPolls = await _context.polls.CountAsync(cancellationToken);

        var totalVotes = await _context.Votes.CountAsync(cancellationToken);

        var totalQuestions = await _context.Questions.CountAsync(cancellationToken);

        var polls = await _context.polls
            .Select(p => new Contracts.Results.Responses.PollResponse(
                p.Id,
                p.Title,
                p.Votes.Count()
            ))
            .ToListAsync(cancellationToken);

        var response = new OverviewResponse(totalPolls, totalVotes, totalQuestions, polls);

        return Result<OverviewResponse>.Success(response);
    }


    public async Task<Result<IEnumerable<VotePerDayResponse>>> GetVotesPerDayAsync(CancellationToken cancellationToken = default)
    {
        var votesPerDay = await _context.Votes
        .GroupBy(v => v.SubmittedOn.Date)
        .Select(g => new
        {
            Date = g.Key,
            TotalVotes = g.Count()
        })
        .OrderBy(x => x.Date)
        .ToListAsync(cancellationToken);

        var response = votesPerDay
            .Select(x => new VotePerDayResponse(
                DateOnly.FromDateTime(x.Date),
                x.TotalVotes
            ));

        return Result<IEnumerable<VotePerDayResponse>>.Success(response);
    }


    public async Task<Result<IEnumerable<TopPollResponse>>> GetTopPollsAsync(CancellationToken cancellationToken = default)
    {
        var topPolls = await _context.polls
        .Select(p => new
        {
            p.Id,
            p.Title,
            TotalVotes = p.Votes.Count()
        })
        .OrderByDescending(p => p.TotalVotes)
        .ThenBy(p => p.Id)
        .Take(5)
        .Select(p => new TopPollResponse(
            p.Id,
            p.Title,
            p.TotalVotes
        ))
        .ToListAsync(cancellationToken);

        return Result<IEnumerable<TopPollResponse>>.Success(topPolls);
    }
}
