using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using Chalkable.StiConnector.Connectors.Model;
using Newtonsoft.Json;

namespace Chalkable.StiConnector.Connectors
{
    public class ReportConnector : ConnectorBase
    {
        public ReportConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public byte[] ProgressReport(ProgressReportParams ps)
        {
            var url = string.Format(BaseUrl + "reports/progress");
            var res = Download(url, ps);
            return res;
        }

        public IList<StudentProgressReportComment> GetProgressReportComments(int sectionId, int? gradingPeriodId = null)
        {
            var url = string.Format(BaseUrl + "chalkable/sections/{0}/progressreportcomments", sectionId);
            var nvc = new NameValueCollection();
            if(gradingPeriodId.HasValue)
                nvc.Add("gradingPeriodId", gradingPeriodId.Value.ToString());
            return Call<IList<StudentProgressReportComment>>(url, nvc);
        }

        public void UpdateProgressReportComment(int sectionId, IList<StudentProgressReportComment> comments)
        {
            var url = string.Format(BaseUrl + "chalkable/sections/{0}/progressreportcomments", sectionId);
            Put(url, comments);
        }

        public byte[] GradebookReport(GradebookReportParams ps)
        {
            var url = string.Format(BaseUrl + "reports/gradebook");
            return Download(url, ps);
        }

        public byte[] WorksheetReport(WorksheetReportParams ps)
        {
            var url = string.Format(BaseUrl + "reports/worksheet");
            return Download(url, ps);
        }

        public byte[] ComprehensiveProgressReport(ComprehensiveProgressParams ps)
        {
            var url = string.Format(BaseUrl + "reports/ComprehensiveProgress");
            return Download(url, ps);
        }

        public byte[] MissingAssignmentsReport(MissingAssignmentsParams ps)
        {
            var url = string.Format(BaseUrl + "reports/missingassignments");
            return Download(url, ps);
        }
    }
}