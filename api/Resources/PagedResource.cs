namespace api.Resources
{
    public class PagedResource<T>
    {
        public IEnumerable<Resource<T>> Items { get; set; } = Enumerable.Empty<Resource<T>>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public Dictionary<string, Link> Links { get; set; } = new();
    }
}
