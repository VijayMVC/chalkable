using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
        private const string SORT = "sort";
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

        
        public IList<Activity> GetActivities(int sectionId, int? start, int? end, int? sort, DateTime? endDate = null, DateTime? startDate = null, bool? complete = false)
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
            if (sort.HasValue)
                optionalParams.Add(SORT, sort.Value.ToString());
            return  Call<IList<Activity>>(url, optionalParams);
        }

        public IList<Activity> GetTeacherActivities(int acadSessionId, int teacherId, int? sort, int? start = null, int? end = null
            , DateTime? endDate = null, DateTime? startDate = null, bool? complete = false)
        {
            var url = string.Format(BaseUrl + "Chalkable/{0}/teachers/{1}/activities", acadSessionId, teacherId);
            var optionalParams = new NameValueCollection();
            if(start.HasValue)
                optionalParams.Add(START_PARAM, start.Value.ToString());
            if(end.HasValue)
                optionalParams.Add(END_PARAM, end.Value.ToString());
            if(startDate.HasValue)
                optionalParams.Add(START_DATE_PARAM, startDate.Value.ToString(Constants.DATE_FORMAT));
            if(endDate.HasValue)
                optionalParams.Add(END_DATE_PARAM, endDate.Value.ToString(Constants.DATE_FORMAT));
            if(complete.HasValue)
                optionalParams.Add(COOMPLETE, complete.Value.ToString());
            if (sort.HasValue)
                optionalParams.Add(SORT, sort.Value.ToString());
            return Call<IList<Activity>>(url, optionalParams);
        }

        public IList<Activity> GetStudentAcivities(int acadSessionId, int studentId, int? sort, int? start = null, int? end = null
            , DateTime? endDate = null, DateTime? startDate = null, bool? complete = false, bool? graded = null, int? classId = null)
        {
            var url = $"{BaseUrl}Chalkable/{acadSessionId}/students/{studentId}/activities";
            var optionalParams = new NameValueCollection();
            if (start.HasValue)
                optionalParams.Add(START_PARAM, start.Value.ToString());
            if (end.HasValue)
                optionalParams.Add(END_PARAM, end.Value.ToString());
            if (startDate.HasValue)
                optionalParams.Add(START_DATE_PARAM, startDate.Value.ToString(Constants.DATE_FORMAT));
            if (endDate.HasValue)
                optionalParams.Add(END_DATE_PARAM, endDate.Value.ToString(Constants.DATE_FORMAT));
            if(complete.HasValue)
                optionalParams.Add(COOMPLETE, complete.Value.ToString());
            if(graded.HasValue)
                optionalParams.Add(GRADED, graded.Value.ToString());
            if(classId.HasValue)
                optionalParams.Add(SECTION_ID, classId.Value.ToString());
            if (sort.HasValue)
                optionalParams.Add(SORT, sort.Value.ToString());
            return Call<IList<Activity>>(url, optionalParams);
        }

        public void CompleteActivity(int activityId, bool complete)
        {
            var url = $"{BaseUrl}chalkable/activities/{activityId}/complete/{complete}";
            Put<Object>(url, null);
        }

        public void CompleteStudentActivities(int acadSessionId, int studentId, int? sectionId, bool complete, DateTime? startDate, DateTime? endDate)
        {
            var url = $"{BaseUrl}chalkable/{acadSessionId}/students/{studentId}/activities/complete/{complete}";
            var nvc = new NameValueCollection();
            if (IsSupportedApiVersion("7.1.0.0"))
            {
                if (startDate.HasValue)
                    nvc.Add(nameof(startDate), startDate.Value.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture));
                if (endDate.HasValue)
                    nvc.Add(nameof(endDate), endDate.Value.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture));
                if (sectionId.HasValue)
                    nvc.Add(nameof(sectionId), sectionId.Value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                if (endDate.HasValue)
                    nvc.Add("date", endDate.Value.ToString(Constants.DATE_FORMAT));
            }
            Post<Object>(url, null, nvc, HttpMethod.Put);
        }

        public void CompleteTeacherActivities(int acadSessionId, int teacherId, int? sectionId, bool complete, DateTime? startDate, DateTime? endDate)
        {
            var url = $"{BaseUrl}chalkable/{acadSessionId}/teachers/{teacherId}/activities/complete/{complete}";
            var nvc = new NameValueCollection();
            if (IsSupportedApiVersion("7.1.0.0"))
            {
                if (startDate.HasValue)
                    nvc.Add(START_DATE_PARAM, startDate.Value.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture));
                if (endDate.HasValue)
                    nvc.Add(END_DATE_PARAM, endDate.Value.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture));
                if (sectionId.HasValue)
                    nvc.Add(SECTION_ID, sectionId.Value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                if (endDate.HasValue)
                    nvc.Add("date", endDate.Value.ToString(Constants.DATE_FORMAT));
            }
            
            Post<Object>(url, null, nvc, HttpMethod.Put);
        }

        public IList<Activity> GetActivitiesByIds(IList<int> ids)
        {
            var nvc = new NameValueCollection();
            var urlBilder = new StringBuilder();
            urlBilder.Append(BaseUrl + "chalkable/activities");
            if (ids != null && ids.Count > 0)
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    nvc.Add($"ids[{i}]", ids[i].ToString());
                }
            }
            return Call<IList<Activity>>(urlBilder.ToString(), nvc);
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

        public IList<ActivityCopyResult> CopyActivity(int id, IList<int> copyToSectionIds)
        {
            var url = string.Format(urlFormat + "/copy", id);
            var nvc = new NameValueCollection();
            if (copyToSectionIds != null && copyToSectionIds.Count > 0)
            {
                for (int i = 0; i < copyToSectionIds.Count; i++)
                    nvc.Add($"copyToSectionIds[{i}]", copyToSectionIds[i].ToString());    
            }
            return Post<IList<ActivityCopyResult>>(url, null, nvc);
        }

        public IList<ActivityCopyResult> CopyActivities(ActivityCopyOptions activityCopyOptions)
        {
            return Post<IList<ActivityCopyResult>, ActivityCopyOptions>($"{BaseUrl}chalkable/activities/copy", activityCopyOptions);
        } 
    }
}