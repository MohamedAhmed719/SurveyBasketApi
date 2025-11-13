namespace SurveyBasket.Api.Contracts.Common;

public class RequestFilters
{
    public int PageSize { get; init; } = 10;
    public int PageNumber { get; init; } = 1;
    public string? SearchValue { get; init; } 
    public string ? SortColumn { get; init; }
    public string SortDirection { get; init; } = "asc";
}
