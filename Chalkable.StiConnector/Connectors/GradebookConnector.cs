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
    }
}
