using System.Web;
using Newtonsoft.Json;

namespace Chalkable.Api.SampleApp.Models
{
    public class DefaultJsonViewData
    {
        public HtmlString JsonData { get; set; }

        public static DefaultJsonViewData Create(object initOptions)
        {

            return new DefaultJsonViewData
            {
                JsonData = new HtmlString(JsonConvert.SerializeObject(initOptions, Formatting.Indented))
            };
        }
    }
}