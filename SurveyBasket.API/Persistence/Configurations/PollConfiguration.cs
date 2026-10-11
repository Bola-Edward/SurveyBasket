namespace SurveyBasket.API.Persistence.Configurations;

public class PollConfiguration : IEntityTypeConfiguration<Poll>
{
    public void Configure(EntityTypeBuilder<Poll> builder)
    {
        builder.HasIndex(p => p.Title)
            .IsUnique();

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Summary)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
