﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class StudentConnector : ConnectorBase
    {
        public StudentConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public IList<StudentCondition> GetStudentConditions(int studentId)
        {
            var url = string.Format("{0}chalkable/students/{1}/conditions", BaseUrl, studentId);
            return Call<IList<StudentCondition>>(url);
        }

        public NowDashboard GetStudentNowDashboard(int acadSessionId, int studentId)
        {
            var url = string.Format("{0}chalkable/{1}/students/{2}/dashboard/now", BaseUrl, acadSessionId, studentId);
            return Call<NowDashboard>(url);
        }
    }
}