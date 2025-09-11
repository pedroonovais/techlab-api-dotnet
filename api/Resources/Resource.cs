namespace api.Resources
{
    public class Resource<T>
    {
        public T Data { get; set; }
        public Dictionary<string, Link> Links { get; set; } = new();
    }
}
