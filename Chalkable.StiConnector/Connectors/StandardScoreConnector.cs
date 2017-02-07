using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class StandardScoreConnector : ConnectorBase
    {
        public StandardScoreConnector(ConnectorLocator locator) : base(locator)
        {
        }
        
        public IList<StandardScore> GetStandardScores(int sectionId, int? standardId, int? gradingPeriodId)
        {
            var url = $"{BaseUrl}chalkable/sections/{sectionId}/standards/students";
            var nvc = new NameValueCollection();
            if(standardId.HasValue)
                nvc.Add("standardId", standardId.Value.ToString());
            if(gradingPeriodId.HasValue)
                nvc.Add("gradingPeriodId", gradingPeriodId.Value.ToString());
            return Call<IList<StandardScore>>(url, nvc);
        }
        
        public StandardScore GetStandardScore(int sectionId, int studentId, int standardId, int gradingPeriodId)
        {
            var url = $"{BaseUrl}chalkable/sections/{sectionId}/standards/{standardId}/students/{studentId}/gradingPeriods/{gradingPeriodId}";
            return Call<StandardScore>(url);
        }
        
        public StandardScore Update(int sectionId, int studentId, int standardId, int gradingPeriodId, StandardScore standardScore)
        {
            var url = $"{BaseUrl}chalkable/sections/{sectionId}/standards/{standardId}/students/{studentId}/gradingPeriods/{gradingPeriodId}";
            return Post(url, standardScore, null, HttpMethod.Put);
        }
    }
}
