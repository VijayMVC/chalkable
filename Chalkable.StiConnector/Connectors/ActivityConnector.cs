using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class ActivityConnector : ConnectorBase
    {
        private string urlFormat;
        public ActivityConnector(ConnectorLocator locator) : base(locator)
        {
            urlFormat = BaseUrl + "Chalkable/activities"; //"http://localhost/Api/chalkable/sections/{0}/activities"; //
        }

        public Activity GetActivity(int sectionId, int id)
        {
            string url = string.Format(urlFormat + "/{1}", sectionId, id);
            return Call<Activity>(url);
        }

        public IList<Activity> GetActivities(int sectionId, int? start, int? end, DateTime? endDate = null, DateTime? startDate = null)
        {
            string url = string.Format(urlFormat, sectionId);
            var optionalParams = new NameValueCollection();
            if(endDate.HasValue)
                optionalParams.Add("endDate", endDate.Value.ToString());
            if(startDate.HasValue)
                optionalParams.Add("startDate", startDate.Value.ToString());
            var res =  Call<IList<Activity>>(url, optionalParams);
            if (start.HasValue)
                res = res.Skip(start.Value).ToList();
            if (end.HasValue)
                res = res.Take(end.Value).ToList();
            return res;
        } 

        public IList<Activity> GetTeacherActivities(int acadSessionId, int teacherId, int? sectionId, int? start = null, int? end = null, DateTime? endDate = null, DateTime? startDate = null)
        {
            string url = string.Format(BaseUrl + "Chalkable/{0}/teachers/{1}/activities", acadSessionId, teacherId);
            var optinalParams = new NameValueCollection();
            if (sectionId.HasValue)
                optinalParams.Add("sectionId", sectionId.Value.ToString());
            if(start.HasValue)
                optinalParams.Add("start", start.Value.ToString());
            if(end.HasValue)
                optinalParams.Add("end", end.Value.ToString());
            if(startDate.HasValue)
                optinalParams.Add("startDate", startDate.Value.ToString("yyyy-MM-dd"));
            if(endDate.HasValue)
                optinalParams.Add("endDate", endDate.Value.ToString("yyyy-MM-dd"));
            return Call<IList<Activity>>(url, optinalParams);
        }

        public IList<Activity> GetStudentAcivities(int acadSessionId, int studentId, int? sectionId, int? start = null, int? end = null, DateTime? endDate = null, DateTime? startDate = null)
        {
            string url = string.Format(BaseUrl + "Chalkable/{0}/students/{1}/activities", acadSessionId, studentId);
            var optinalParams = new NameValueCollection();
            if (sectionId.HasValue)
                optinalParams.Add("sectionId", sectionId.Value.ToString());
            if (start.HasValue)
                optinalParams.Add("start", start.Value.ToString());
            if (end.HasValue)
                optinalParams.Add("end", end.Value.ToString());
            if (startDate.HasValue)
                optinalParams.Add("startDate", startDate.Value.ToString());
            if (endDate.HasValue)
                optinalParams.Add("endDate", endDate.Value.ToString());
            return Call<IList<Activity>>(url, optinalParams);
        } 

        public void DeleteActivity(int sectionId, int id)
        {
            string url = string.Format(urlFormat + "/{1}", sectionId, id);
            Post<Activity>(url, null, DELETE);
            
        }
        public Activity CreateActivity(int sectionId, Activity activity)
        {
            string url = string.Format(urlFormat, sectionId);
            return Post(url, activity);
        }
        public void UpdateActivity(int sectionId, int id, Activity activity)
        {
            string url = string.Format(urlFormat + "/{1}", sectionId, id);
            Post(url, activity, PUT);
        }
    }
}