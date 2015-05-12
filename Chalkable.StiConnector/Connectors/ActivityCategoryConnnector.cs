using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Chalkable.StiConnector.Connectors.Model;


namespace Chalkable.StiConnector.Connectors
{
    public class ActivityCategoryConnnector : ConnectorBase
    {
        public ActivityCategoryConnnector(ConnectorLocator locator) : base(locator)
        {
        }

        public ActivityCategory Add(ActivityCategory activityCategory)
        {
           return Post(string.Format("{0}activitycategories", BaseUrl), activityCategory);
        }

        public void Update(int activityCategoryId, ActivityCategory activityCategory)
        {
            Put(string.Format("{0}activities/categories/{1}", BaseUrl, activityCategoryId), activityCategory);
        }
        public void Delete(int activityCategoryId)
        {
            Delete(string.Format("{0}activities/categories/{1}", BaseUrl, activityCategoryId));
        }
        public ActivityCategory GetById(int activityCategoryId)
        {
            return Call<ActivityCategory>(string.Format("{0}activities/categories/{1}", BaseUrl, activityCategoryId));
        }
        public IList<ActivityCategory> GetBySectionIds(IList<int> sectionIds)
        {
            if(sectionIds.Count == 1)
                return Call<IList<ActivityCategory>>(string.Format("{0}sections/{1}/activities/categories", BaseUrl, sectionIds.First()));
   
            var nvc = new NameValueCollection();
            for (int i = 0; i < sectionIds.Count; i++)
                nvc.Add(string.Format("sectionIds[{0}]", i), sectionIds[i].ToString());
           return Post<IList<ActivityCategory>, int[]>(string.Format("{0}activities/categories", BaseUrl), sectionIds.ToArray());
            //return Post<IList<ActivityCategory>, int[]>(string.Format("{0}/sections/activities/categories", BaseUrl), sectionIds.ToArray());
        }


   }
}
