using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using Chalkable.Common;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class ActivityConnector : ConnectorBase
    {
        private const string END_DATE_PARAM = "endDate";
        private const string START_DATE_PARAM = "startDate";
        private const string START_PARAM = "start";
        private const string END_PARAM = "end";
        private const string COOMPLETE = "complete";
        private const string GRADED = "graded";
        private const string SECTION_ID = "sectionId";
        private string urlFormat;
        public ActivityConnector(ConnectorLocator locator) : base(locator)
        {
            urlFormat = BaseUrl + "Chalkable/activities/{0}";
        }

        public Activity GetActivity(int id)
        {
            string url = string.Format(urlFormat, id);
            return Call<Activity>(url);
        }

        private IList<Activity> PaginateActivity(IList<Activity> activities, int? start, int? end)
        {
            if (start.HasValue)
                activities = activities.Skip(start.Value).ToList();
            if (end.HasValue)
                activities = activities.Take(end.Value - (start ?? 0)).ToList();
            return activities;
        } 

        public IList<Activity> GetActivities(int sectionId, int? start, int? end, DateTime? endDate = null, DateTime? startDate = null, bool? complete = false)
        {
            var url = string.Format(BaseUrl + "chalkable/sections/{0}/activities", sectionId);
            var optionalParams = new NameValueCollection();
            if (start.HasValue)
                optionalParams.Add(START_PARAM, start.Value.ToString());
            if (end.HasValue)
                optionalParams.Add(END_PARAM, end.Value.ToString());
            if(endDate.HasValue)
                optionalParams.Add(END_DATE_PARAM, endDate.Value.ToString(Constants.DATE_FORMAT));
            if(startDate.HasValue)
                optionalParams.Add(START_DATE_PARAM, startDate.Value.ToString(Constants.DATE_FORMAT));
            if(complete.HasValue)
                optionalParams.Add(COOMPLETE, complete.Value.ToString());
            return  Call<IList<Activity>>(url, optionalParams);
        } 

        public IList<Activity> GetTeacherActivities(int acadSessionId, int teacherId, int? start = null, int? end = null
            , DateTime? endDate = null, DateTime? startDate = null, bool? complete = false)
        {
            var url = string.Format(BaseUrl + "Chalkable/{0}/teachers/{1}/activities", acadSessionId, teacherId);
            var optinalParams = new NameValueCollection();
            if(start.HasValue)
                optinalParams.Add(START_PARAM, start.Value.ToString());
            if(end.HasValue)
                optinalParams.Add(END_PARAM, end.Value.ToString());
            if(startDate.HasValue)
                optinalParams.Add(START_DATE_PARAM, startDate.Value.ToString(Constants.DATE_FORMAT));
            if(endDate.HasValue)
                optinalParams.Add(END_DATE_PARAM, endDate.Value.ToString(Constants.DATE_FORMAT));
            if(complete.HasValue)
                optinalParams.Add(COOMPLETE, complete.Value.ToString());
            return Call<IList<Activity>>(url, optinalParams);
        }

        public IList<Activity> GetStudentAcivities(int acadSessionId, int studentId, int? start = null, int? end = null
            , DateTime? endDate = null, DateTime? startDate = null, bool? complete = false, bool? graded = null, int? classId = null)
        {
            var url = string.Format(BaseUrl + "Chalkable/{0}/students/{1}/activities", acadSessionId, studentId);
            var optinalParams = new NameValueCollection();
            if (start.HasValue)
                optinalParams.Add(START_PARAM, start.Value.ToString());
            if (end.HasValue)
                optinalParams.Add(END_PARAM, end.Value.ToString());
            if (startDate.HasValue)
                optinalParams.Add(START_DATE_PARAM, startDate.Value.ToString(Constants.DATE_FORMAT));
            if (endDate.HasValue)
                optinalParams.Add(END_DATE_PARAM, endDate.Value.ToString(Constants.DATE_FORMAT));
            if(complete.HasValue)
                optinalParams.Add(COOMPLETE, complete.Value.ToString());
            if(graded.HasValue)
                optinalParams.Add(GRADED, graded.Value.ToString());
            if(classId.HasValue)
                optinalParams.Add(SECTION_ID, classId.Value.ToString());
            return Call<IList<Activity>>(url, optinalParams);
        }

        public void CompleteActivity(int activityId, bool complete)
        {
            var url = string.Format(BaseUrl + "chalkable/activities/{0}/complete/{1}", activityId, complete);
            Put<Object>(url, null);
        }

        public void DeleteActivity(int id)
        {
            Delete(string.Format(urlFormat, id));           
        }
        public Activity CreateActivity(int sectionId, Activity activity)
        {
            activity.SectionId = sectionId;
            return Post(BaseUrl + "Chalkable/activities", activity);
        }
        public void UpdateActivity(int id, Activity activity)
        {
            Put(string.Format(urlFormat, id), activity);
        }

        public void CopyActivity(int id, IList<int> copyToSectionIds)
        {
            var url = string.Format(urlFormat + "/copy", id);
            var nvc = new NameValueCollection();
            if (copyToSectionIds != null && copyToSectionIds.Count > 0)
            {
                for (int i = 0; i < copyToSectionIds.Count; i++)
                    nvc.Add(string.Format("copyToSectionIds[{0}]", i), copyToSectionIds[i].ToString());    
            }
            Post<Object>(url, null, nvc);
        }
    }
}