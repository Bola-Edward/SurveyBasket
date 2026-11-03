using SurveyBasket.API.Contracts.Question.Responses;

namespace SurveyBasket.API.Services.Interfaces;

public interface IQuestionService
{
    Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, CancellationToken cancellationToken = default);
    Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default);
    Task<Result<QuestionResponse>> GetByIdAsync(int pollId, int questionId, CancellationToken cancellation = default);
    Task<Result> UpdateAsync(int pollId, int questionId, QuestionRequest request, CancellationToken cancellationToken = default);
    Task<Result> ToggleStatusAsync(int pollId, int questionId, CancellationToken cancellationToken = default);
}