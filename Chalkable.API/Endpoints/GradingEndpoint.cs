using System.Net;
using System.Threading.Tasks;

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
            var endpoint = $"{url}?announcementApplicationId={announcementApplicationId}&studentId={studentId}&gradeValue={gradeValue}";
            return await Connector.Call<bool>(endpoint, method: WebRequestMethods.Http.Post);
        } 
    }
}
