using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class StudentConnector : ConnectorBase
    {
        public StudentConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public IList<StudentCondition> GetStudentConditions(int studentId)
        {
            var url = string.Format("{0}chalkable/students/{1}/conditions", BaseUrl, studentId);
            return Call<IList<StudentCondition>>(url);
        }

        public NowDashboard GetStudentNowDashboard(int acadSessionId, int studentId)
        {
            var url = string.Format("{0}chalkable/{1}/students/{2}/dashboard/now", BaseUrl, acadSessionId, studentId);
            return Call<NowDashboard>(url);
        }

        public StudentExplorerDashboard GetStudentExplorerDashboard(int acadSessionId, int studentId, DateTime? date = null)
        {
            var nvc = new NameValueCollection();
            if(date.HasValue)
                nvc.Add("date", date.Value.ToString(Constants.DATE_FORMAT));
            var url = string.Format("{0}chlkable/{1}/students/{2}/dashboard/explorer", BaseUrl, acadSessionId, studentId);
            return Call<StudentExplorerDashboard>(url, nvc);
        }
    }
}
