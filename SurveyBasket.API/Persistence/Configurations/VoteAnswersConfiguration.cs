namespace SurveyBasket.API.Persistence.Configurations;

public class VoteAnswersConfiguration : IEntityTypeConfiguration<VoteAnswer>
{
    public void Configure(EntityTypeBuilder<VoteAnswer> builder)
    {
        builder.HasKey(va => va.Id);

        builder.HasOne(va => va.Vote)
            .WithMany(v => v.VoteAnswers)
            .HasForeignKey(va => va.VoteId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(va => va.Question)
            .WithMany()
            .HasForeignKey(va => va.QuestionId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(va => va.Answer)
            .WithMany()
            .HasForeignKey(va => va.AnswerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(va => new { va.VoteId, va.QuestionId })
            .IsUnique();
    }
}
