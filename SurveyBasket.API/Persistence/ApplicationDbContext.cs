
namespace SurveyBasket.API.Persistence;

public class ApplicationDbContext(DbContextOptions options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Poll> polls { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
