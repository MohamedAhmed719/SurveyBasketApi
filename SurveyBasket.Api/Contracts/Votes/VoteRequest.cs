namespace SurveyBasket.Api.Contracts.Votes;

public record VoteRequest(
    IEnumerable<VoteAnserRequest> Answers
    );