﻿using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentAppsViewData
    {
        public ShortPersonViewData Person { get; set; }
        public PaginatedList<BaseApplicationViewData> Applications { get; set; }

        public static StudentAppsViewData Create(StudentDetails student, PaginatedList<BaseApplicationViewData> apps
            , IList<StudentCustomAlertDetail> customAlerts, IList<StudentHealthCondition> healthConditions)
        {
            return new StudentAppsViewData
            {
                Applications = apps,
                Person = StudentProfileViewData.Create(student, customAlerts, healthConditions)
            };
        }
    }
}