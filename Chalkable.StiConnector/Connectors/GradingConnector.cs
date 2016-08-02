using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class GradingConnector : ConnectorBase
    {
        public GradingConnector(ConnectorLocator locator)
            : base(locator)
        {
        }
        
        public GradingSummaryDashboard GetStudentGradingSummary(int acadSessionId, int studentId)
        {
            return Call<GradingSummaryDashboard>(BuildBaseUrl(acadSessionId, studentId) + "summary");
            
        }
        
        public GradingDetailsDashboard GetStudentGradingDetails(int acadSessionId, int studentId, int gradingPeriodId)
        {
            var url = BuildBaseUrl(acadSessionId, studentId) + "detail/" + gradingPeriodId;
            return Call<GradingDetailsDashboard>(url);
        }
        
        private string BuildBaseUrl(int acadSessionId, int studentId)
        {
            return $"{BaseUrl}chalkable/{acadSessionId}/students/{studentId}/dashboard/grading/";
        }
    }
}
