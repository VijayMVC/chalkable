using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class GradingConnector : ConnectorBase
    {
        public GradingConnector(ConnectorLocator locator)
            : base(locator)
        {
        }
        
        public async Task<GradingSummaryDashboard> GetStudentGradingSummary(int acadSessionId, int studentId)
        {
            return await CallAsync<GradingSummaryDashboard>(BuildBaseUrl(acadSessionId, studentId) + "summary"); 
        }
        
        public async Task<GradingDetailsDashboard> GetStudentGradingDetails(int acadSessionId, int studentId, int gradingPeriodId)
        {
            var url = BuildBaseUrl(acadSessionId, studentId) + "detail/" + gradingPeriodId;
            return await CallAsync<GradingDetailsDashboard>(url);
        }
        
        private string BuildBaseUrl(int acadSessionId, int studentId)
        {
            return $"{BaseUrl}chalkable/{acadSessionId}/students/{studentId}/dashboard/grading/";
        }
    }
}
