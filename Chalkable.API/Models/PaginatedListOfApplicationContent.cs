using System.Collections.Generic;

namespace Chalkable.API.Models
{
    public class PaginatedListOfApplicationContent
    {
        public IList<ApplicationContent> ApplicationContents { get; set; }
        public int TotalCount { get; set; }
    }

    public class ApplicationContent
    {
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public string ContentId { get; set; }
    }
}
