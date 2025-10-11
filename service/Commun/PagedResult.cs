namespace service.Common
{
    public record PagedResult<T>(
        IReadOnlyList<T> Items,
        int TotalItems,
        int PageNumber,
        int PageSize
    )
    {
        public int TotalPages => (int)System.Math.Ceiling(TotalItems / (double)PageSize);
    }
}
