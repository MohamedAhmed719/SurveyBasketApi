namespace SurveyBasket.Api.Entites;

public sealed class Vote
{
    public int Id { get; set; }
    public DateTime SubmittedOn { get; set; } = DateTime.UtcNow;
    public string UserId { get; set; } 
    public int PollId { get; set; }
    public ApplicationUser User { get; set; } = default!;
    public Poll Poll { get; set; } = default!;
    public ICollection<VoteAnswer> VoteAnswers { get; set; } = [];
}
