using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
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

        public Gradebook Calculate(int sectionId, int? gradingPeriodId = null)
        {
            var nvc = new NameValueCollection();
            if(gradingPeriodId.HasValue)
                nvc.Add("gradingPeriodId", gradingPeriodId.Value.ToString());
            return Post<Gradebook>(string.Format(url_format + "/calculate", sectionId), null, nvc);
        }

        public Gradebook GetBySectionAndGradingPeriod(int sectionId, int? categoryId = null,
                                                       int? gradingPeriodId = null, int? standardId = null)
        {
            var nvc = new NameValueCollection();
            if(categoryId.HasValue)
                nvc.Add("categoryId", categoryId.Value.ToString());
            if(gradingPeriodId.HasValue)
                nvc.Add("gradingPeriodId", gradingPeriodId.Value.ToString());
            if(standardId.HasValue)
                nvc.Add("standardId", standardId.Value.ToString());
            return Call<Gradebook>(string.Format(url_format, sectionId), nvc);
        }

        public IList<string> GetGradebookComments(int acadSessionId, int teacherId)
        {
            return Call<IList<string>>(string.Format(BaseUrl + "chalkable/{0}/teachers/{1}/comments", acadSessionId, teacherId));
        } 

        public StudentAverage UpdateStudentAverage(int sectionId, StudentAverage studentAverage)
        {
            var url = string.Format(BaseUrl + "chalkable/sections/{0}/averages/{1}/students/{2}"
                                    , sectionId, studentAverage.AverageId, studentAverage.StudentId);
            return Put(url, studentAverage);
        }

        public IList<StudentAverage> GetStudentAverages(int sectionId, int? gradingPeriodId)
        {
            var url = string.Format(BaseUrl + "chalkable/sections/{0}/averages/students", sectionId);
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

        public IList<SectionGradesSummary> GetSectionGradesSummary(IList<int> sectionIds, int gradingPeriodId)
        {
            var nvc = new NameValueCollection {{"gradingPeriodId", gradingPeriodId.ToString()}};
            for (int i = 0; i < sectionIds.Count; i++)
                nvc.Add(string.Format("sectionIds[{0}]", i.ToString()), sectionIds[i].ToString());
            return Call<IList<SectionGradesSummary>>(string.Format("{0}chalkable/grades/summary", BaseUrl), nvc);
        }
        
    }
}
