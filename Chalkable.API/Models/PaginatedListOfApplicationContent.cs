using System.Collections.Generic;
using Newtonsoft.Json;

namespace Chalkable.API.Models
{
    public class PaginatedListOfApplicationContent
    {
        public IList<ApplicationContent> ApplicationContents { get; set; }
        public int TotalCount { get; set; }
    }

    public class ApplicationContent
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("imageurl")]
        public string ImageUrl { get; set; }

        [JsonProperty("contentid")]
        public string ContentId { get; set; }
    }
}
