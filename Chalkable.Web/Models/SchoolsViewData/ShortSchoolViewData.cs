using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models.SchoolsViewData
{
    public class ShortSchoolViewData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int StatusNumber { get; set; }
        public string Status { get; set; }
        public string TimeZoneId { get; set; }
        public string DemoPrefix { get; set; }

        protected ShortSchoolViewData(School school)
        {
            Id = school.Id;
            Name = school.Name;
            StatusNumber = (int)school.Status;
            //todo: remove this
            if (statusDic.ContainsKey(school.Status))
                Status = statusDic[school.Status];
            TimeZoneId = school.TimeZone;
            DemoPrefix = school.DemoPrefix;
        }
        public static ShortSchoolViewData Create(School school)
        {
            return new ShortSchoolViewData(school);
        }
        public static ShortSchoolViewData Create(Guid id, string name, string timeZone, SchoolStatus status, string prefix)
        {
            return Create(new School
            {
                Id = id,
                Name = name,
                DemoPrefix = prefix,
                TimeZone = timeZone,
                Status = status
            });
        }
        private static IDictionary<SchoolStatus, string> statusDic = new
            Dictionary<SchoolStatus, string>
            {
                {SchoolStatus.PersonalInfoImported, ChlkResources.SCHOOL_PERSONAL_INFO_IMPORTED},
                {SchoolStatus.Created, ChlkResources.SCHOOL_CREATED},
                {SchoolStatus.TeacherLogged, ChlkResources.SCHOOL_TEACHER_LOGGED_IN},
                {SchoolStatus.DataImported, ChlkResources.SCHOOL_SOME_DATA_IMPORTED_SUCCESSFULLY},
                {SchoolStatus.GradeLevels, ChlkResources.SCHOOL_DATA_GRADELEVELS},
                {SchoolStatus.MarkingPeriods, ChlkResources.SCHOOL_DATA_MARKING_PERIODS },
                {SchoolStatus.BlockScheduling, ChlkResources.SCHOOL_DATA_BLOCK_SCHEDULING},
                {SchoolStatus.DailyPeriods, ChlkResources.SCHOOL_DATA_DAILY_PERIODS },
                {SchoolStatus.InvitedUser, ChlkResources.SCHOOL_INVITED_USER },
                {SchoolStatus.InvitedStudent, ChlkResources.SCHOOL_STUDENT_WAS_INVITED },
                {SchoolStatus.ScheduleInfoImported, ChlkResources.SCHOOL_SCHEDULE_INFO_IMPORTED },
                {SchoolStatus.StudentLogged,  ChlkResources.SCHOOL_STUDENT_LOGGED_IN},
                {SchoolStatus.PayingCustomer, ChlkResources.SCHOOL_PAYING_CUSTOMER}
            };

    }
}