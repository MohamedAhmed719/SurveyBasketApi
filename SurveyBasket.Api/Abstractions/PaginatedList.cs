using System.Threading.Tasks;

namespace SurveyBasket.Api.Abstractions;

public class PaginatedList<T>(List<T> items,int pageNumber, int pageSize, int count)
{
    public List<T> Items { get; private set; } = items;
    public int PageNumber { get; private set; } = pageNumber;
    public int PageSize { get; private set; } = pageSize;
    public int Count { get; private set; } = count;
    public int TotalPags => (int)Math.Ceiling((double)Count / PageSize);
    public bool HasNextPages => PageNumber < TotalPags;
    public bool HasPreviousPage => PageNumber > 1;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source,int pageNumber,int pageSize)
    {
        var count = await source.CountAsync();

        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedList<T>(items, pageNumber, pageSize, count);

    }

}
