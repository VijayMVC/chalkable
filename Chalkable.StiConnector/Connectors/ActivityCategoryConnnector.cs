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

        public IList<ActivityCategory> GetBySectionId(int sectionId)
        {
            return Call<IList<ActivityCategory>>(string.Format("{0}sections/{1}/activities/categories", BaseUrl, sectionId));
        } 

        public IList<ActivityCategory> GetBySectionIds(List<int> sectionIds)
        {
            var nvc = new NameValueCollection();
            if (sectionIds != null && sectionIds.Count > 0)
            {
                for (int i = 0; i < sectionIds.Count; i++)
                    nvc.Add(string.Format("sectionIds[{0}]", i), sectionIds[i].ToString());   
            }
            return Call<IList<ActivityCategory>>(string.Format("{0}activities/categories"), nvc);
        }
    }
}
