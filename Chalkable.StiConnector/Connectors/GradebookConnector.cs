using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class GradebookConnector : ConnectorBase
    {
        private string url_format;
        public GradebookConnector(ConnectorLocator locator) : base(locator)
        {
            url_format = BaseUrl + "chalkable/sections/{0}/gradebook";
        }

        
        public async Task<Gradebook> Calculate(int sectionId, int? gradingPeriodId)
        {
            var nvc = new NameValueCollection();
            if(gradingPeriodId.HasValue)
                nvc.Add("gradingPeriodId", gradingPeriodId.Value.ToString());
            return await PostAsync<Gradebook>(string.Format(url_format + "/calculate", sectionId), null, nvc);
        }

        
        public async Task<Gradebook> GetBySectionAndGradingPeriod(int sectionId, int? categoryId = null,
                                                       int? gradingPeriodId = null, int? standardId = null)
        {
            var nvc = new NameValueCollection();
            if(categoryId.HasValue)
                nvc.Add("categoryId", categoryId.Value.ToString());
            if(gradingPeriodId.HasValue)
                nvc.Add("gradingPeriodId", gradingPeriodId.Value.ToString());
            if(standardId.HasValue)
                nvc.Add("standardId", standardId.Value.ToString());
            return await CallAsync<Gradebook>(string.Format(url_format, sectionId), nvc);
        }

        
        public IList<string> GetGradebookComments(int acadSessionId, int teacherId)
        {
            return Call<IList<string>>(string.Format(BaseUrl + "chalkable/{0}/teachers/{1}/comments", acadSessionId, teacherId));
        }

        
        public StudentAverage UpdateStudentAverage(int sectionId, StudentAverage studentAverage)
        {
            var url =$"{BaseUrl}chalkable/sections/{sectionId}/averages/{studentAverage.AverageId}/students/{studentAverage.StudentId}";
            return Put(url, studentAverage);
        }

        
        public IList<StudentAverage> GetStudentAverages(int sectionId, int? gradingPeriodId)
        {
            var url = $"{BaseUrl}chalkable/sections/{sectionId}/averages/students";
            var nvc = new NameValueCollection();
            if(gradingPeriodId.HasValue)
                nvc.Add("gradingPeriodId", gradingPeriodId.Value.ToString());
            return Call<IList<StudentAverage>>(url, nvc);
        }

        
        public void PostGrades(int sectionId, int? gradingPeriodId)
        {
            var nvc = new NameValueCollection();
            if(gradingPeriodId.HasValue)
                nvc.Add("gradingPeriodId", gradingPeriodId.Value.ToString());
            Post<Object, Object>(string.Format(url_format + "/postgrades", sectionId), new Object(), nvc);
        }

        
        public async Task<IList<SectionGradesSummary>> GetSectionGradesSummary(IList<int> sectionIds, int gradingPeriodId)
        {
            var nvc = new NameValueCollection {{"gradingPeriodId", gradingPeriodId.ToString()}};
            for (int i = 0; i < sectionIds.Count; i++)
                nvc.Add($"sectionIds[{i}]", sectionIds[i].ToString());
            return await CallAsync<IList<SectionGradesSummary>>($"{BaseUrl}chalkable/grades/summary", nvc);
        }


        
        public async Task<AverageDashboard> GetAveragesDashboard(int sectionId, int? gradingPeriodId)
        {
            var url = $"{BaseUrl}chalkable/sections/{sectionId}/averages/dashboard";
            var nvc = new NameValueCollection();
            if(gradingPeriodId.HasValue)
                nvc.Add("gradingPeriodId", gradingPeriodId.Value.ToString());
            return await CallAsync<AverageDashboard>(url, nvc);
        }

        
        public void PostStandards(int sectionId, int? gradingPeriodId)
        {
            var nvc = new NameValueCollection();
            if (gradingPeriodId.HasValue)
                nvc.Add("gradingPeriodId", gradingPeriodId.Value.ToString());
            Post<Object, Object>($"{BaseUrl}chalkable/sections/{sectionId}/gradebook/poststandards", new Object(), nvc);
        }
    }
}
