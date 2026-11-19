namespace SurveyBasket.API.Mapping;

public class MappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<QuestionRequest, Question>().Ignore(nameof(Question.Answers));

        config.NewConfig<Contracts.Authentication.Requests.RegisterRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email);
    }
}
