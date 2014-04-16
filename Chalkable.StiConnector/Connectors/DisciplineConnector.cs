using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class DisciplineConnector : ConnectorBase
    {
        private string urlFormat;
        public DisciplineConnector(ConnectorLocator locator) : base(locator)
        {
            urlFormat = BaseUrl + "chalkable/sections/{0}/disciplinereferrals/{1}";
        }

        public DisciplineReferral Create(DisciplineReferral discipline)
        {
            if(!discipline.SectionId.HasValue)
                throw new ChalkableException("Invalid sectionId param");
            return Post(BuildUrl(discipline.SectionId.Value, discipline.Date), discipline);
        }

        public void Update(DisciplineReferral discipline)
        {
            if (!discipline.SectionId.HasValue)
                throw new ChalkableException("Invalid sectionId param");
            Put(BuildUrl(discipline.SectionId.Value, discipline.Date), discipline);
        }

        public void Delete(int id, int sectionId, DateTime date)
        {
            Delete(BuildUrl(sectionId, date, id));
        }

        public DisciplineReferral GetById(int id, int sectionId, DateTime date)
        {
            return Call<DisciplineReferral>(BuildUrl(sectionId, date, id));
        }

        public IList<DisciplineReferral> GetList(int sectionId, DateTime date)
        {
            return Call<IList<DisciplineReferral>>(BuildUrl(sectionId, date));
        }

        private string BuildUrl(int sectionId, DateTime date, int? id = null)
        {
            var res = string.Format(urlFormat, sectionId, date.ToString(Constants.DATE_FORMAT));
            if (id.HasValue)
                res += "/" + id.Value.ToString();
            return res;
        }
    }
}
