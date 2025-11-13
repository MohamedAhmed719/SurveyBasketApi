namespace SurveyBasket.Api.Contracts.Votes;

public record VoteAnserRequest(
    int QuestionId,
    int AnswerId
    );
