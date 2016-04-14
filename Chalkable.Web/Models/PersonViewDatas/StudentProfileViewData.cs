using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentProfileViewData : StudentViewData
    {
        public IList<StudentHealthConditionViewData> HealthConditions { get; set; }
        public IList<StudentCustomAlertDetailViewData> StudentCustomAlertDetails { get; set; }

        protected StudentProfileViewData(StudentDetails student, IList<StudentCustomAlertDetail> customAlerts,
            IList<StudentHealthCondition> healthConditions) : base(student)
        {
            HealthConditions = StudentHealthConditionViewData.Create(healthConditions);
            StudentCustomAlertDetails = StudentCustomAlertDetailViewData.Create(customAlerts);
        }

        public static StudentProfileViewData Create(StudentDetails student, IList<StudentCustomAlertDetail> customAlerts,
            IList<StudentHealthCondition> healthConditions)
        {
            return new StudentProfileViewData(student, customAlerts, healthConditions);
        }
    }
}