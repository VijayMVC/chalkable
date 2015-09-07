﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Master.Model;
using Chalkable.Data.Master.Model.Chlk;

namespace Chalkable.Web.Models.SchoolsViewData
{
    public class SchoolViewData : ShortSchoolViewData
    {
        public string SchoolType { get; set; }
        //public string SchoolUrl { get; set; }
        public bool SendEmailNotifications { get; set; }
        //public int ImportSystemType { get; set; }
        public DateTime? StudyCenterEnabledTill { get; set; }
        public bool IsMessagingDisabled { get; set; }

        protected SchoolViewData(School school) : base(school)
        {
            StudyCenterEnabledTill = school.StudyCenterEnabledTill;
            IsMessagingDisabled = school.IsMessagingDisabled;
        }

        public static new SchoolViewData Create(School school)
        {
            return new SchoolViewData(school);
        }
        public static IList<SchoolViewData> Create(IList<School> schools)
        {
            return schools.Select(Create).ToList();
        } 
    }
}