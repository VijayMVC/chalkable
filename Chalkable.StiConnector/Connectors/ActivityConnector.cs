﻿using System;
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

        public IList<Activity> GetActivities(int sectionId, int? start, int? end, DateTime? endDate = null, DateTime? startDate = null)
        {
            var url = string.Format(BaseUrl + "chalkable/sections/{0}/activities", sectionId);
            var optionalParams = new NameValueCollection();
            if(endDate.HasValue)
                optionalParams.Add(END_DATE_PARAM, endDate.Value.ToString(Constants.DATE_FORMAT));
            if(startDate.HasValue)
                optionalParams.Add(START_DATE_PARAM, startDate.Value.ToString(Constants.DATE_FORMAT));
            var res =  Call<IList<Activity>>(url, optionalParams);
            return PaginateActivity(res, start, end);
        } 

        public IList<Activity> GetTeacherActivities(int acadSessionId, int teacherId, int? start = null, int? end = null, DateTime? endDate = null, DateTime? startDate = null)
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
            var res = Call<IList<Activity>>(url, optinalParams);
            return PaginateActivity(res, start, end);
        }

        public IList<Activity> GetStudentAcivities(int acadSessionId, int studentId, int? start = null, int? end = null, DateTime? endDate = null, DateTime? startDate = null)
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
            var res = Call<IList<Activity>>(url, optinalParams);
            return PaginateActivity(res, start, end);
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
    }
}