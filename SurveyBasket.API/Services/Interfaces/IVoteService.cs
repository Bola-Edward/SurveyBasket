using SurveyBasket.API.Contracts.Vote.Requests;

namespace SurveyBasket.API.Services.Interfaces;

public interface IVoteService
{
    Task<Result> AddAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken = default);
}
