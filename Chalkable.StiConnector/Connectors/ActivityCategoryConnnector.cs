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
           return Post($"{BaseUrl}activitycategories", activityCategory);
        }

        public void Update(int activityCategoryId, ActivityCategory activityCategory)
        {
            Put($"{BaseUrl}activities/categories/{activityCategoryId}", activityCategory);
        }
        public void Delete(int activityCategoryId)
        {
            Delete($"{BaseUrl}activities/categories/{activityCategoryId}");
        }
        public ActivityCategory GetById(int activityCategoryId)
        {
            return Call<ActivityCategory>($"{BaseUrl}activities/categories/{activityCategoryId}");
        }
        public IList<ActivityCategory> GetBySectionIds(IList<int> sectionIds)
        {
            if(sectionIds.Count == 1)
                return Call<IList<ActivityCategory>>($"{BaseUrl}sections/{sectionIds.First()}/activities/categories");
   
            var nvc = new NameValueCollection();
            for (int i = 0; i < sectionIds.Count; i++)
                nvc.Add($"sectionIds[{i}]", sectionIds[i].ToString());
           return Post<IList<ActivityCategory>, int[]>($"{BaseUrl}activities/categories", sectionIds.ToArray());
            //return Post<IList<ActivityCategory>, int[]>(string.Format("{0}/sections/activities/categories", BaseUrl), sectionIds.ToArray());
        }

        public IList<ActivityCategoryCopyResult> CopyCategories(int fromSectionId, ActivityCategoryCopyOption copyOption)
        {
            return Post<IList<ActivityCategoryCopyResult>, ActivityCategoryCopyOption>($"{BaseUrl}sections/{fromSectionId}/activities/categories/copy", copyOption);
        } 
   }
}
