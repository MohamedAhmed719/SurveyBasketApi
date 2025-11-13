namespace SurveyBasket.Api.Contracts.Authentication;

public record ConfirmEmailRequest(string userId,string Code);