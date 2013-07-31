using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class SchoolInfoViewData : SchoolViewData
    {
        public IList<string> Emails { get; set; }
        public string Status { get; set; }
        public int StatusNumber { get; set; }
        public int[] Buttons { get; set; }
        public SisSyncViewData SisData { get; set; }

        protected SchoolInfoViewData(School school, SisSync sisData) : base(school)
        {
            StatusNumber = (int) school.Status;
            Status = statusBtView[school.Status].Status;
            Buttons = statusBtView[school.Status].Buttons;
            SisData = SisSyncViewData.Create(sisData);
        }

        public static SchoolInfoViewData Create(School school, SisSync sisData)
        {
            return new SchoolInfoViewData(school, sisData);
        }

        private class SchoolStatusButtonViewData
        {
            public string Status { get; set; }
            public int[] Buttons { get; set; }
        }
        private static IDictionary<SchoolStatus, SchoolStatusButtonViewData> statusBtView = new
            Dictionary<SchoolStatus, SchoolStatusButtonViewData>
            {
                {SchoolStatus.PersonalInfoImported, new SchoolStatusButtonViewData
                        {
                            Status = ChlkResources.SCHOOL_PERSONAL_INFO_IMPORTED,
                            Buttons = new[] { 2, 1 }
                        }
                },
                {SchoolStatus.Created, new SchoolStatusButtonViewData
                        {
                            Status = ChlkResources.SCHOOL_CREATED,
                            Buttons = new[] { 1 }
                        }
                },
                {SchoolStatus.TeacherLogged, new SchoolStatusButtonViewData { Status = ChlkResources.SCHOOL_TEACHER_LOGGED_IN}},
                {SchoolStatus.DataImported, new SchoolStatusButtonViewData { Status = ChlkResources.SCHOOL_SOME_DATA_IMPORTED_SUCCESSFULLY}},
                {SchoolStatus.GradeLevels, new SchoolStatusButtonViewData { Status = ChlkResources.SCHOOL_DATA_GRADELEVELS}},
                {SchoolStatus.MarkingPeriods, new SchoolStatusButtonViewData { Status = ChlkResources.SCHOOL_DATA_MARKING_PERIODS }},
                {SchoolStatus.BlockScheduling, new SchoolStatusButtonViewData {Status = ChlkResources.SCHOOL_DATA_BLOCK_SCHEDULING}},
                {SchoolStatus.DailyPeriods, new SchoolStatusButtonViewData
                        {
                            Status = ChlkResources.SCHOOL_DATA_DAILY_PERIODS,
                            Buttons = new[] { 3, 2, 1 }
                        }
                },
                {SchoolStatus.InvitedUser, new SchoolStatusButtonViewData { Status = ChlkResources.SCHOOL_INVITED_USER }},
                {SchoolStatus.InvitedStudent, new SchoolStatusButtonViewData { Status = ChlkResources.SCHOOL_STUDENT_WAS_INVITED }},
                {SchoolStatus.ScheduleInfoImported, new SchoolStatusButtonViewData { Status = ChlkResources.SCHOOL_SCHEDULE_INFO_IMPORTED }},
                {SchoolStatus.StudentLogged, new SchoolStatusButtonViewData
                        {
                            Status = ChlkResources.SCHOOL_STUDENT_LOGGED_IN,
                            Buttons = new[] { 4, 3, 2, 1 }
                        }
                },
                {SchoolStatus.PayingCustomer, new SchoolStatusButtonViewData
                        {
                            Status = ChlkResources.SCHOOL_PAYING_CUSTOMER,
                            Buttons = new[] {1 , 2 }
                        }
                }
            }; 
    }
}