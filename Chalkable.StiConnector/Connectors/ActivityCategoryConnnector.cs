using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;


namespace Chalkable.StiConnector.Connectors
{
    public class ActivityCategoryConnnector : ConnectorBase
    {
        public ActivityCategoryConnnector(ConnectorLocator locator) : base(locator)
        {
        }

        public IList<ActivityCategory> GetBySectionIds(IList<int> sectionIds)
        {
            if(sectionIds.Count == 1)
                return Call<IList<ActivityCategory>>(string.Format("{0}sections/{1}/activities/categories", BaseUrl, sectionIds.First()));
   
            var nvc = new NameValueCollection();
            for (int i = 0; i < sectionIds.Count; i++)
                nvc.Add(string.Format("sectionIds[{0}]", i), sectionIds[i].ToString());   
            return Call<IList<ActivityCategory>>(string.Format("{0}activities/categories", BaseUrl), nvc);
        }
    }
}
