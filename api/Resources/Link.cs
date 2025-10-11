namespace api.Resources
{
    public class Link
    {
        public string Href { get; set; }
        public string Method { get; set; }

        public Link(string href, string method)
        {
            Href = href;
            Method = method;
        }
    }
}
