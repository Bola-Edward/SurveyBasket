namespace SurveyBasket.API.Persistence.Configurations;

public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasIndex(a => new { a.QuestionId, a.Content })
            .IsUnique()
            .HasFilter("[IsActive] = 1");

        builder.Property(a => a.Content)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(a => a.IsActive)
            .HasDefaultValue(true);

        builder.HasQueryFilter(a => a.IsActive);

        builder.HasOne(a => a.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}