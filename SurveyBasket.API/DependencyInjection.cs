
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
        services.AddAuthConfiguration();

        services.AddScoped<IAuthService, AuthService>();
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
            .AddValidatorsFromAssemblyContaining<PollRequestValidator>();
        return services;
    }


    public static IServiceCollection AddAuthConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IJWTProvider, JWTProvider>();

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        }).AddJwtBearer(o =>
        {
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ggEcWxr6SjhBSgO6zk7SagMjqy2bC2xR")),
                ValidIssuer = "SurveyBasketApp",
                ValidAudience = "SurveyBasketApp users"
            };
        });

        return services;
    }
}