namespace SurveyBasket.API.Persistence.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasKey(q => q.Id);

        builder.HasIndex(x => new { x.PollId, x.Content })
            .IsUnique()
            .HasFilter("[IsActive] = 1");

        builder.Property(q => q.Content)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(q => q.IsActive)
            .HasDefaultValue(true);

        builder.HasQueryFilter(q => q.IsActive);

        builder.HasOne(q => q.Poll)
            .WithMany(p => p.Questions)
            .HasForeignKey(q => q.PollId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedBy)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
