using SurveyBasket.API.Services.Implementations;

namespace SurveyBasket.API;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPollService, PollService>();

        return services;
    }
}