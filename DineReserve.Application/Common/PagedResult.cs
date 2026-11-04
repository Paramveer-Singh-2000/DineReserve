
namespace DineReserve.Application.Common;

public class PagedResult<T>
{
    public PagedResult(
        IEnumerable<T> items,
        int totalCount,
        int pageSize,
        int pageNumber)
    {
        Items = items;
        TotalItemCount = totalCount;

        TotalPages = pageSize > 0 && totalCount > 0
            ? (int)Math.Ceiling(totalCount / (double)pageSize)
            : 0;

        var offset = (long)(pageNumber - 1) * pageSize;

        if (totalCount <= 0 ||
            pageSize <= 0 ||
            pageNumber < 1 ||
            offset >= totalCount)
        {
            ItemFrom = 0;
            ItemTo = 0;
        }
        else
        {
            ItemFrom = (int)offset + 1;
            ItemTo = (int)Math.Min(
                totalCount,
                offset + pageSize);
        }
    }

    public IEnumerable<T> Items { get; }
    public int TotalPages { get; }
    public int TotalItemCount { get; }
    public int ItemFrom { get; }
    public int ItemTo { get; }
}
