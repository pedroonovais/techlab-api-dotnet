using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace api.Resources
{
    public static class PaginationLinkBuilder
    {
        public static string BuildUrl(HttpRequest request, int pageNumber, int pageSize)
        {
            var url = $"{request.Path}";
            var queryDict = new Dictionary<string, string?>();

            foreach (var kv in request.Query)
                queryDict[kv.Key] = kv.Value;

            queryDict["pageNumber"] = pageNumber.ToString();
            queryDict["pageSize"] = pageSize.ToString();

            var qs = QueryString.Create(queryDict.Where(kv => kv.Value is not null)
                                                 .Select(kv => new KeyValuePair<string, string?>(kv.Key, kv.Value)));
            return url + qs.ToUriComponent();
        }
    }
}
