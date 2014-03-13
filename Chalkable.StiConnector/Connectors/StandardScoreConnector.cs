using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
            var url = string.Format("chalkable/sections/{0}/standards/students", sectionId);
            var nvc = new NameValueCollection();
            if(standardId.HasValue)
                nvc.Add("standardId", standardId.Value.ToString());
            if(gradingPeriodId.HasValue)
                nvc.Add("gradingPeriodId", gradingPeriodId.Value.ToString());
            return Call<IList<StandardScore>>(url, nvc);
        } 

        public StandardScore GetStandardScore(int sectionId, int studentId, int standardId, int gradingPeriodId)
        {
            var url = string.Format("chalkable/sections/{0}/standards/{1}/students/{2}/gradingPeriods/{3}"
                                    , sectionId, standardId, studentId, gradingPeriodId);
            return Call<StandardScore>(url);
        }

        public StandardScore Update(int sectionId, int studentId, int standardId, int gradingPeriodId,
                                    StandardScore standardScore)
        {
            var url = string.Format("chalkable/sections/{0}/standards/{1}/students/{2}/gradingPeriods/{3}"
                                    , sectionId, standardId, standardId, gradingPeriodId);
            return Post(url, standardScore, null, HttpMethod.Put);
        }
    }
}
