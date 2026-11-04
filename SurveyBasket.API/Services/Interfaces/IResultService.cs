using SurveyBasket.API.Contracts.Results.Responses;

namespace SurveyBasket.API.Services.Interfaces;

public interface IResultService
{
    public Task<Result<PollResultResponse>> GetPollResultsAsync(int pollId, CancellationToken cancellationToken = default);
    public Task<Result<OverviewResponse>> GetOverviewAsync(CancellationToken cancellationToken = default);
    public Task<Result<IEnumerable<VotePerDayResponse>>> GetVotesPerDayAsync(CancellationToken cancellationToken = default);
    public Task<Result<IEnumerable<TopPollResponse>>> GetTopPollsAsync(CancellationToken cancellationToken = default);
}
