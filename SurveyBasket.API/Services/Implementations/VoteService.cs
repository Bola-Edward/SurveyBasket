using SurveyBasket.API.Contracts.Vote.Requests;

namespace SurveyBasket.API.Services.Implementations;

public class VoteService(ApplicationDbContext context) : IVoteService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result> AddAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Check if the user has already voted in this poll
        var hasVote = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken);

        if (hasVote)
            return Result.Failure(VoteErrors.AlreadyVoted);


        // 2. Check if the poll exists and is currently available
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var pollIsAvailable = await _context.polls
            .AnyAsync(
                x => x.Id == pollId &&
                     x.IsPublished &&
                     x.StartsAt <= today &&
                     x.EndsAt >= today,
                cancellationToken);

        if (!pollIsAvailable)
            return Result.Failure(PollErrors.NotFound);


        // 3. Get all active questions belonging to this poll
        var availableQuestionIds = await _context.Questions
            .Where(x => x.PollId == pollId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);


        // 4. Check that the user submitted an answer for every question
        var submittedAnswers = request.VoteAnswers.ToList();

        var submittedQuestionIds = submittedAnswers
            .Select(x => x.QuestionId)
            .ToHashSet();

        if (!submittedQuestionIds.SetEquals(availableQuestionIds))
            return Result.Failure(VoteErrors.InvalidQuestions);


        // 5. Validate that each answer belongs to its submitted question
        var submittedAnswerIds = submittedAnswers
            .Select(x => x.AnswerId)
            .ToList();

        var validAnswers = await _context.Answers
            .Where(a => submittedAnswerIds.Contains(a.Id) &&
                        a.Question.PollId == pollId)
            .Select(a => new
            {
                a.Id,
                a.QuestionId
            })
            .ToListAsync(cancellationToken);

        var validAnswerPairs = validAnswers
            .Select(a => (a.QuestionId, a.Id))
            .ToHashSet();

        var hasInvalidAnswers = submittedAnswers.Any(
            x => !validAnswerPairs.Contains((x.QuestionId, x.AnswerId)));

        if (hasInvalidAnswers)
            return Result.Failure(VoteErrors.InvalidAnswers);


        // 6. Create the vote with its answers
        var vote = new Vote
        {
            PollId = pollId,
            UserId = userId,
            VoteAnswers = submittedAnswers
                .Adapt<List<VoteAnswer>>()
        };

        await _context.Votes.AddAsync(vote, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
