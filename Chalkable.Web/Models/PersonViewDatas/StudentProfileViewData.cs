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
            if(healthConditions != null)
                HealthConditions = StudentHealthConditionViewData.Create(healthConditions);
            if(customAlerts != null)
                StudentCustomAlertDetails = StudentCustomAlertDetailViewData.Create(customAlerts);
        }

        public static StudentProfileViewData Create(StudentDetails student, IList<StudentCustomAlertDetail> customAlerts,
            IList<StudentHealthCondition> healthConditions)
        {
            return new StudentProfileViewData(student, customAlerts, healthConditions);
        }
    }
}