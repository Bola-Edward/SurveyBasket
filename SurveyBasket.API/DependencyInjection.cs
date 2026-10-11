namespace SurveyBasket.API;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
        services.AddControllers();
        services.AddSwaggerServices();
        services.AddMapasterConfiguration();
        services.AddFluentValidationConfiguration();
        services.AddScoped<IPollService, PollService>();
        return services;
    }


    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddOpenApi();
        return services;
    }

    public static IServiceCollection AddMapasterConfiguration(this IServiceCollection services)
    {
        TypeAdapterConfig.GlobalSettings.Scan(typeof(PollMappingConfig).Assembly);
        return services;
    }

    public static IServiceCollection AddFluentValidationConfiguration(this IServiceCollection services)
    {
        // To Add Any Other Validators
        services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssemblyContaining<CreatePollRequestValidator>();
        return services;
    }
}