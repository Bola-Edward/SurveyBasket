
namespace SurveyBasket.API.Persistence;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{


    public DbSet<Poll> polls { get; set; }
}
