using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return Post(string.Format(urlFormat, discipline.SectionId, discipline.Date), discipline);
        }

        public void Update(DisciplineReferral discipline)
        {
            if (!discipline.SectionId.HasValue)
                throw new ChalkableException("Invalid sectionId param");
            Put(string.Format(urlFormat, discipline.SectionId, discipline.Date), discipline);
        }

        public void Delete(int id, int sectionId, DateTime date)
        {
            Delete(string.Format(urlFormat + "/{2}", sectionId, date, id));
        }

        public DisciplineReferral GetById(int id, int sectionId, DateTime date)
        {
            return Call<DisciplineReferral>(string.Format(urlFormat + "/{2}", sectionId, date, id));
        }

        public IList<DisciplineReferral> GetList(int sectionId, DateTime date)
        {
            return Call<IList<DisciplineReferral>>(string.Format(urlFormat, sectionId, date));
        }
    }
}
