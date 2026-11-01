using SurveyBasket.API.Contracts.Question.Responses;

namespace SurveyBasket.API.Services.Implementations;

public class QuestionService(ApplicationDbContext context) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken)
    {
        var pollIsExist = await _context.polls.AnyAsync(p => p.Id == pollId, cancellationToken);

        if (!pollIsExist)
            return Result<IEnumerable<QuestionResponse>>.Failure(QuestionErrors.PollNotFound);

        var questions = await _context.Questions.Where(q => q.PollId == pollId)
                                          .Include(q => q.Answers)
                                          .AsNoTracking()
                                          .ToListAsync(cancellationToken);

        var response = questions.Adapt<List<QuestionResponse>>();

        return Result<IEnumerable<QuestionResponse>>.Success(response);
    }

    public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken)
    {
        var pollIsExist = await _context.polls.AnyAsync(p => p.Id == pollId, cancellationToken);

        if (!pollIsExist)
            return Result<QuestionResponse>.Failure(QuestionErrors.PollNotFound);

        var questionIsExist = await _context.Questions.AnyAsync(q => q.Content == request.Content && q.PollId == pollId && q.IsActive, cancellationToken);

        if (questionIsExist)
            return Result<QuestionResponse>.Failure(QuestionErrors.DuplicateContent);

        var question = request.Adapt<Question>();
        question.PollId = pollId;

        question.Answers = request.Answers.Select(answer => new Answer { Content = answer }).ToList();

        await _context.Questions.AddAsync(question, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<QuestionResponse>.Success(question.Adapt<QuestionResponse>());
    }


    public async Task<Result<QuestionResponse>> GetByIdAsync(int pollId, int questionId, CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions
        .Where(q => q.PollId == pollId && q.Id == questionId && q.IsActive)
        .Include(q => q.Answers.Where(a => a.IsActive))
        .FirstOrDefaultAsync(cancellationToken);

        if (question is null)
            return Result<QuestionResponse>.Failure(QuestionErrors.NotFound);

        return Result<QuestionResponse>.Success(question.Adapt<QuestionResponse>());
    }

    public async Task<Result> UpdateAsync(int pollId, int questionId, QuestionRequest request, CancellationToken cancellationToken)
    {
        var question = await _context.Questions
            .IgnoreQueryFilters()
            .Include(x => x.Answers)
            .SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == questionId, cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.NotFound);

        var questionIsExists = await _context.Questions
            .AnyAsync(x =>
                x.PollId == pollId &&
                x.Id != questionId &&
                x.Content == request.Content, cancellationToken);

        if (questionIsExists)
            return Result.Failure(QuestionErrors.DuplicateContent);

        question.Content = request.Content;

        // Get current answers
        var currentAnswers = question.Answers.Select(x => x.Content).ToList();

        // Find new answers
        var newAnswers = request.Answers.Except(currentAnswers, StringComparer.OrdinalIgnoreCase).ToList();

        // Add new answers
        newAnswers.ForEach(answer =>
        {
            question.Answers.Add(new Answer { Content = answer });
        });

        // Activate or deactivate answers
        question.Answers.ToList().ForEach(answer =>
        {
            answer.IsActive = request.Answers.Contains(answer.Content, StringComparer.OrdinalIgnoreCase);
        });

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }


    public async Task<Result> ToggleStatusAsync(int pollId, int questionId, CancellationToken cancellationToken)
    {
        var question = await _context.Questions
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == questionId, cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.NotFound);

        question.IsActive = !question.IsActive;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
