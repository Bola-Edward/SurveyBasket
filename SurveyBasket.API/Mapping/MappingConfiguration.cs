namespace SurveyBasket.API.Mapping;

public class MappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<QuestionRequest, Question>().Ignore(nameof(Question.Answers));
    }
}
