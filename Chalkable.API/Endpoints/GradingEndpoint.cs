using System.Threading.Tasks;
using System.Web;

namespace Chalkable.API.Endpoints
{
    public class GradingEndpoint : Base
    {
        public GradingEndpoint(IConnector connector) : base(connector)
        {
        }
        
        public async Task<bool> SetAutoGrade(int announcementApplicationId, int studentId, string gradeValue)
        {
            var url = "/Grading/SetAutoGrade.json";
            //var postData = new NameValueCollection
            //{
            //    ["announcementApplicationId"] = announcementApplicationId.ToString(),
            //    ["studentId"] = studentId.ToString(),
            //    ["gradeValue"] = gradeValue
            //};

            var postData = HttpUtility.ParseQueryString(string.Empty);

            postData.Add("announcementApplicationId", announcementApplicationId.ToString());
            postData.Add("studentId", studentId.ToString());
            postData.Add("gradeValue", gradeValue);

            return await Connector.Post<bool>(url, postData);
        } 
    }
}
