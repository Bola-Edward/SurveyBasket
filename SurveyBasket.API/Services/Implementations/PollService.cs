using SurveyBasket.API.Services.Interfaces;

namespace SurveyBasket.API.Services.Implementations;

public class PollService : IPollService
{
    private static readonly List<Poll> _polls = new List<Poll>
    {
        new Poll {Id = 1, Title = "first poll", Description = "my poll"}
    };


    public IEnumerable<Poll> GetAll()
    {
        return _polls;
    }

    public Poll? GetById(int id)
    {
        return _polls.SingleOrDefault(p => p.Id == id);
    }

    public Poll Add(Poll poll)
    {
        poll.Id = _polls.Count + 1;
        _polls.Add(poll);
        return poll;
    }

    public bool Update(int id, Poll poll)
    {
        var currentPoll = GetById(id);

        if (currentPoll is null)
            return false;

        currentPoll.Title = poll.Title;
        currentPoll.Description = poll.Description;

        return true;
    }

    public bool Delete(int id)
    {
        var poll = GetById(id);

        if (poll is null)
            return false;

        _polls.Remove(poll);
        return true;
    }
}
