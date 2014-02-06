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
        private const string SECTION_ID_PARAM = "sectionId";

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
                optionalParams.Add(END_DATE_PARAM, endDate.Value.ToString(Constants.DATE_FORMAT));
            if(startDate.HasValue)
                optionalParams.Add(START_DATE_PARAM, startDate.Value.ToString(Constants.DATE_FORMAT));
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
                optinalParams.Add(SECTION_ID_PARAM, sectionId.Value.ToString());
            if(start.HasValue)
                optinalParams.Add(START_PARAM, start.Value.ToString());
            if(end.HasValue)
                optinalParams.Add(END_PARAM, end.Value.ToString());
            if(startDate.HasValue)
                optinalParams.Add(START_DATE_PARAM, startDate.Value.ToString(Constants.DATE_FORMAT));
            if(endDate.HasValue)
                optinalParams.Add(END_DATE_PARAM, endDate.Value.ToString(Constants.DATE_FORMAT));
            return Call<IList<Activity>>(url, optinalParams);
        }

        public IList<Activity> GetStudentAcivities(int acadSessionId, int studentId, int? sectionId, int? start = null, int? end = null, DateTime? endDate = null, DateTime? startDate = null)
        {
            string url = string.Format(BaseUrl + "Chalkable/{0}/students/{1}/activities", acadSessionId, studentId);
            var optinalParams = new NameValueCollection();
            if (sectionId.HasValue)
                optinalParams.Add(SECTION_ID_PARAM, sectionId.Value.ToString());
            if (start.HasValue)
                optinalParams.Add(START_PARAM, start.Value.ToString());
            if (end.HasValue)
                optinalParams.Add(END_PARAM, end.Value.ToString());
            if (startDate.HasValue)
                optinalParams.Add(START_DATE_PARAM, startDate.Value.ToString(Constants.DATE_FORMAT));
            if (endDate.HasValue)
                optinalParams.Add(END_DATE_PARAM, endDate.Value.ToString(Constants.DATE_FORMAT));
            return Call<IList<Activity>>(url, optinalParams);
        } 

        public void DeleteActivity(int sectionId, int id)
        {
            Delete(string.Format(urlFormat + "/{1}", sectionId, id));           
        }
        public Activity CreateActivity(int sectionId, Activity activity)
        {
            return Post(string.Format(urlFormat, sectionId), activity);
        }
        public void UpdateActivity(int sectionId, int id, Activity activity)
        {
            Put(string.Format(urlFormat + "/{1}", sectionId, id), activity);
        }
    }
}