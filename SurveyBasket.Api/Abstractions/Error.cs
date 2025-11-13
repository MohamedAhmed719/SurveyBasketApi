namespace SurveyBasket.Api.Abstractions;

public record Error(string Code,string Description,int? statusCode)
{
    public static Error none = new(string.Empty, string.Empty,null);
}
