namespace SurveyBasket.API.Persistence.Configurations;

public class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.SubmittedOn)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(v => v.Poll)
            .WithMany(p => p.Votes)
            .HasForeignKey(v => v.PollId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(v => v.User)
            .WithMany()
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(v => v.VoteAnswers)
            .WithOne(va => va.Vote)
            .HasForeignKey(va => va.VoteId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(v => new { v.PollId, v.UserId })
            .IsUnique();
    }
}
